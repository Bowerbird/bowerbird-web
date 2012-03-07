﻿/// <reference path="Scripts/jquery-1.7.js" />
/// <reference path="Scripts/jQuery.tmpl.js" />
/// <reference path="Scripts/jquery.cookie.js" />
/// <reference path="Chat.ui.js" />

(function ($, connection, window, ui, utility) {
    "use strict";

    var chat = connection.chat,
        messageHistory = [],
        historyLocation = 0,
        originalTitle = document.title,
        unread = 0,
        isUnreadMessageForUser = false,
        focus = true,
        loadingHistory = false,
        checkingStatus = false,
        typing = false,
        typingTimeoutId = null,
        $ui = $(ui),
        messageSendingDelay = 1500,
        pendingMessages = {};

    function isSelf(user) {
        return chat.name === user.Name;
    }

    function populateRoom(room) {
        var d = $.Deferred();
        // Populate the list of users rooms and messages 
        chat.getRoomInfo(room)
                .done(function (roomInfo) {
                    $.each(roomInfo.Users, function () {
                        var userViewModel = getUserViewModel(this);
                        ui.addUser(userViewModel, room);
                        ui.setUserActivity(userViewModel);
                    });

                    $.each(roomInfo.Owners, function () {
                        ui.setRoomOwner(this, room);
                    });

                    $.each(roomInfo.RecentMessages, function () {
                        var viewModel = getMessageViewModel(this);

                        ui.addChatMessage(viewModel, room);
                    });

                    ui.changeRoomTopic(roomInfo);
                    // mark room as initialized to differentiate messages
                    // that are added after initial population
                    ui.setInitialized(room);
                    ui.scrollToBottom(room);

                    d.resolveWith(chat);
                })
                .fail(function () {
                    d.rejectWith(chat);
                });

        return d;
    }

    function populateLobbyRooms() {
        // Populate the user list with room names
        chat.getRooms()
            .done(function (rooms) {
                ui.populateLobbyRooms(rooms);
            });
    }

    function scrollIfNecessary(callback, room) {
        var nearEnd = ui.isNearTheEnd(room);

        callback();

        if (nearEnd) {
            ui.scrollToBottom(room);
        }
    }

    function getUserViewModel(user, isOwner) {
        var lastActive = user.LastActivity.fromJsonDate();
        return {
            name: user.Name,
            id: user.Id,
            owner: isOwner,
            active: user.Active,
            lastActive: lastActive,
            timeAgo: $.timeago(lastActive)
        };
    }

    function getMessageViewModel(message) {
        var re = new RegExp("\\b@?" + chat.name.replace(/\./, '\\.') + "\\b", "i");
        return {
            name: message.User.Name,
            hash: message.User.Hash,
            message: message.Content,
            id: message.Id,
            date: message.When.fromJsonDate(),
            highlight: re.test(message.Content) ? 'highlight' : '',
            isOwn: re.test(message.User.name)
        };
    }

    // Save some state in a cookie
    function updateCookie() {
        var state = {
            userId: chat.id,
            activeRoom: chat.activeRoom,
            preferences: ui.getState()
        },
        jsonState = window.JSON.stringify(state);

        $.cookie('bowerbird.state', jsonState, { path: '/', expires: 30 });
    }

    function updateTitle() {
        if (unread === 0) {
            document.title = originalTitle;
        }
        else {
            document.title =
                (isUnreadMessageForUser ? '*' : '')
                + '(' + unread + ') ' + originalTitle;
        }
    }

    function updateUnread(room, isMentioned) {
        if (focus === false) {
            isUnreadMessageForUser = (isUnreadMessageForUser || isMentioned);

            unread = unread + 1;
        } else {
            //we're currently focused so remove
            //the * notification
            isUnreadMessageForUser = false;
        }

        ui.updateUnread(room, isMentioned);

        updateTitle();
    }

    // Room commands

    // When the /join command gets raised this is called
    chat.joinRoom = function (room) {
        var added = ui.addRoom(room.Name);
        ui.setActiveRoom(room.Name);
        if (room.Private) {
            ui.setRoomLocked(room.Name);
        }

        if (added) {
            populateRoom(room.Name).done(function () {
                ui.addMessage('You just entered ' + room.Name, 'notification', room.Name);
            });
        }
    };

    // Called when a returning users join chat
    chat.logOn = function (rooms) {
        var activeRoom = this.activeRoom,
            loadRooms = function () {
                $.each(rooms, function (index, room) {
                    if (chat.activeRoom !== room.Name) {
                        populateRoom(room.Name);
                    }
                });
                populateLobbyRooms();
            };

        $.each(rooms, function (index, room) {
            ui.addRoom(room.Name);
            if (room.Private) {
                ui.setRoomLocked(room.Name);
            }
        });
        ui.setUserName(chat.name);
        ui.addMessage('Welcome back ' + chat.name, 'notification', 'lobby');
        ui.addMessage('You can join any of the rooms on the right', 'notification', 'lobby');
        ui.addMessage('Type /rooms to list all available rooms', 'notification', 'lobby');
        ui.addMessage('Type /logout to log out of chat', 'notification', 'lobby');

        // Process any urls that may contain room names
        ui.run();

        // If the active room didn't change then set the active room (since no navigation happened)
        if (activeRoom === this.activeRoom) {
            ui.setActiveRoom(this.activeRoom || 'Lobby');
        }

        if (this.activeRoom) {
            // Always populate the active room first then load the other rooms so it looks fast :)
            populateRoom(this.activeRoom).done(loadRooms);
        }
        else {
            // There's no active room so we don't care
            loadRooms();
        }
    };

    chat.lockRoom = function (user, room) {
        if (!isSelf(user) && this.activeRoom === room) {
            ui.addMessage(user.Name + ' has locked ' + room + '.', 'notification', this.activeRoom);
        }

        ui.setRoomLocked(room);
    };

    // Called when this user locked a room
    chat.roomLocked = function (room) {
        ui.addMessage(room + ' is now locked.', 'notification', this.activeRoom);
    };

    chat.roomClosed = function (room) {
        populateLobbyRooms();
        ui.addMessage('Room \'' + room + '\' is now closed', 'notification', this.activeRoom);
    };

    chat.addOwner = function (user, room) {
        ui.setRoomOwner(user.Name, room);
    };

    chat.removeOwner = function (user, room) {
        ui.clearRoomOwner(user.Name, room);
    };

    chat.updateRoomCount = function (room, count) {
        ui.updateLobbyRoomCount(room, count);
    };

    chat.markInactive = function (users) {
        $.each(users, function () {
            var viewModel = getUserViewModel(this);
            ui.setUserActivity(viewModel);
        });
    };

    chat.updateActivity = function (user) {
        var viewModel = getUserViewModel(user);
        ui.setUserActivity(viewModel);
    };

    chat.addMessageContent = function (id, content, room) {
        var nearTheEndBefore = ui.isNearTheEnd(room);

        scrollIfNecessary(function () {
            ui.addChatMessageContent(id, content, room);
        }, room);

        updateUnread(room, false /* isMentioned: this is outside normal messages and user shouldn't be mentioned */);

        // Adding external content can sometimes take a while to load
        // Since we don't know when it'll become full size in the DOM
        // we're just going to wait a little bit and hope for the best :) (still a HACK tho)
        window.setTimeout(function () {
            var nearTheEndAfter = ui.isNearTheEnd(room);
            if (nearTheEndBefore && nearTheEndAfter) {
                ui.scrollToBottom();
            }
        }, 850);
    };

    chat.addMessage = function (message, room) {
        var viewModel = getMessageViewModel(message);

        // Update your message when it comes from the server
        if (ui.messageExists(viewModel.id)) {
            ui.replaceMessage(viewModel);
            return;
        }

        scrollIfNecessary(function () {
            ui.addChatMessage(viewModel, room);
        }, room);

        var isMentioned = viewModel.highlight === 'highlight';

        updateUnread(room, isMentioned);
    };

    chat.addUser = function (user, room, isOwner) {
        var viewModel = getUserViewModel(user, isOwner);

        var added = ui.addUser(viewModel, room);

        if (added) {
            if (!isSelf(user)) {
                ui.addMessage(user.Name + ' just entered ' + room, 'notification', room);
            }
        }
    };

    // Called when the server sends a notification message
    chat.postNotification = function (msg, room) {
        ui.addMessage(msg, 'notification', room);
    };

    // Called when you created a new user
    chat.userCreated = function () {
        ui.setUserName(this.name);
        ui.addMessage('Your nick is ' + this.name, 'notification');

        // Process any urls that may contain room names
        ui.run();

        if (!this.activeRoom) {
            // Set the active room to the lobby so the rooms on the right load
            ui.setActiveRoom('Lobby');
        }

        // Update the cookie
        updateCookie();
    };

    chat.logOut = function (rooms) {
        ui.setActiveRoom('Lobby');

        // Close all rooms
        $.each(rooms, function () {
            ui.removeRoom(this);
        });

        ui.addMessage("You've been logged out.", 'notification', this.activeRoom);

        chat.activeRoom = undefined;
        chat.name = undefined;
        chat.id = undefined;

        updateCookie();

        // Reload the page
        document.location = document.location.pathname;
    };

    chat.showUserInfo = function (userInfo) {
        var lastActivityDate = userInfo.LastActivity.fromJsonDate();
        var status = "Currently " + userInfo.Status;
        if (userInfo.IsAfk) {
            status += userInfo.Status == 'Active' ? ' but ' : ' and ';
            status += ' is Afk';
        }
        ui.addMessage('User information for ' + userInfo.Name +
            " (" + status + " - last seen " + $.timeago(lastActivityDate) + ")", 'list-header');

        if (userInfo.AfkNote) {
            ui.addMessage('Afk: ' + userInfo.AfkNote, 'list-item');
        }
        else if (userInfo.Note) {
            ui.addMessage('Note: ' + userInfo.Note, 'list-item');
        }

        chat.showUsersOwnedRoomList(userInfo.Name, userInfo.OwnedRooms);
    };

    chat.setTyping = function (user, room) {
        var viewModel = getUserViewModel(user);
        ui.setUserTyping(viewModel, room);
    };

    chat.sendMeMessage = function (name, message, room) {
        ui.addMessage('*' + name + ' ' + message, 'notification', room);
    };

    chat.leave = function (user, room) {
        if (isSelf(user)) {
            ui.setActiveRoom('Lobby');
            ui.removeRoom(room);
            ui.addMessage('You have left ' + room, 'notification');
        }
        else {
            ui.removeUser(user, room);
            ui.addMessage(user.Name + ' left ' + room, 'notification', room);
        }
    };

    // Helpish commands
    chat.showRooms = function (rooms) {
        ui.addMessage('Rooms', 'list-header');
        if (!rooms.length) {
            ui.addMessage('No rooms available', 'list-item');
        }
        else {
            // sort rooms by count descending
            var sorted = rooms.sort(function (a, b) {
                return a.Count > b.Count ? -1 : 1;
            });

            $.each(sorted, function () {
                ui.addMessage(this.Name + ' (' + this.Count + ')', 'list-item');
            });
        }
    };

    chat.showUsersInRoom = function (room, names) {
        ui.addMessage('Users in ' + room, 'list-header');
        if (names.length === 0) {
            ui.addMessage('Room is empty', 'list-item');
        }
        else {
            $.each(names, function () {
                ui.addMessage('- ' + this, 'list-item');
            });
        }
    };

    chat.listUsers = function (users) {
        if (users.length === 0) {
            ui.addMessage('No users matched your search', 'list-header');
        }
        else {
            ui.addMessage('The following users match your search', 'list-header');
            ui.addMessage(users.join(', '), 'list-item');
        }
    };

    chat.showUsersRoomList = function (user, rooms) {
        var status = "Currently " + user.Status;
        if (rooms.length === 0) {
            ui.addMessage(user.Name + ' (' + status + ') is not in any rooms', 'list-header');
        }
        else {
            ui.addMessage(user.Name + ' (' + status + ') is in the following rooms', 'list-header');
            ui.addMessage(rooms.join(', '), 'list-item');
        }
    };

    chat.showUsersOwnedRoomList = function (user, rooms) {
        if (rooms.length === 0) {
            ui.addMessage(user + ' does not own any rooms', 'list-header');
        }
        else {
            ui.addMessage(user + ' owns the following rooms', 'list-header');
            ui.addMessage(rooms.join(', '), 'list-item');
        }
    };

    $ui.bind(ui.events.typing, function () {
        // If not in a room, don't try to send typing notifications
        if (!chat.activeRoom) {
            return;
        }

        if (checkingStatus === false && typing === false) {
            typing = true;

            chat.typing(chat.activeGroupId);

            window.setTimeout(function () {
                typing = false;
            },
            3000);
        }
    });

    $ui.bind(ui.events.sendMessage, function (ev, msg) {
        var id = utility.newId(),
            clientMessage = {
                id: id,
                content: msg,
                room: chat.activeRoom
            },
            messageCompleteTimeout = null;


        if (msg[0] !== '/') {
            // Added the message to the ui first
            var viewModel = {
                name: chat.name,
                hash: chat.hash,
                message: $('<div/>').text(clientMessage.content).html(),
                id: clientMessage.id,
                date: new Date(),
                highlight: ''
            };

            ui.addChatMessage(viewModel, clientMessage.room);

            // If there's a significant delay in getting the message sent
            // mark it as pending
            messageCompleteTimeout = window.setTimeout(function () {
                // If after a second
                ui.markMessagePending(id);
            },
            messageSendingDelay);

            pendingMessages[id] = messageCompleteTimeout;
        }

        chat.send(clientMessage)
            .done(function (requiresUpdate) {
                if (requiresUpdate === true) {
                    ui.showUpdateUI();
                }

                if (messageCompleteTimeout) {
                    clearTimeout(messageCompleteTimeout);
                    delete pendingMessages[id];
                }

                ui.confirmMessage(id);
            })
            .fail(function (e) {
                ui.addMessage(e, 'error');
            });

        // Store message history
        messageHistory.push(msg);

        // REVIEW: should this pop items off the top after a certain length?
        historyLocation = messageHistory.length;
    });

    $ui.bind(ui.events.focusit, function () {
        focus = true;
        unread = 0;
        updateTitle();
    });

    $ui.bind(ui.events.blurit, function () {
        focus = false;

        updateTitle();
    });

    $ui.bind(ui.events.openRoom, function (ev, room) {
        chat.send('/join ' + room, chat.activeRoom)
            .fail(function (e) {
                ui.setActiveRoom('Lobby');
                ui.addMessage(e, 'error');
            });
    });

    $ui.bind(ui.events.closeRoom, function (ev, room) {
        chat.send('/leave ' + room, chat.activeRoom)
            .fail(function (e) {
                ui.addMessage(e, 'error');
            });
    });

    $ui.bind(ui.events.prevMessage, function () {
        historyLocation -= 1;
        if (historyLocation < 0) {
            historyLocation = messageHistory.length - 1;
        }
        ui.setMessage(messageHistory[historyLocation]);
    });

    $ui.bind(ui.events.nextMessage, function () {
        historyLocation = (historyLocation + 1) % messageHistory.length;
        ui.setMessage(messageHistory[historyLocation]);
    });

    $ui.bind(ui.events.activeRoomChanged, function (ev, room) {
        if (room === 'Lobby') {
            populateLobbyRooms();

            // Remove the active room
            chat.activeRoom = undefined;
        }
        else {
            // When the active room changes update the client state and the cookie
            chat.activeRoom = room;
        }

        ui.scrollToBottom(room);
        updateCookie();
    });

    $ui.bind(ui.events.scrollRoomTop, function (ev, roomInfo) {
        // Do nothing if we're loading history already
        if (loadingHistory === true) {
            return;
        }

        loadingHistory = true;

        // TODO: Show a little animation so the user experience looks fancy
        chat.getPreviousMessages(roomInfo.messageId)
            .done(function (messages) {
                ui.prependChatMessages($.map(messages, getMessageViewModel), roomInfo.name);
                loadingHistory = false;
            })
            .fail(function () {
                loadingHistory = false;
            });
    });

    $(ui).bind(ui.events.preferencesChanged, function (ev) {
        updateCookie();
    });

    $(function () {
        var stateCookie = $.cookie('bowerbird.state'),
            state = stateCookie ? JSON.parse(stateCookie) : {};

        // Initialize the ui, passing the user preferences
        ui.initialize(state.preferences);

        ui.addMessage('Welcome to ' + originalTitle, 'notification');
        ui.addMessage('Type /help to see the list of commands', 'notification');

        connection.hub.start(function () {
            chat.join()
                .fail(function (e) {
                    ui.addMessage(e, 'error');
                })
                .done(function (success) {
                    if (success === false) {
                        ui.showLogin();
                        ui.addMessage('Type /login to show the login screen', 'notification');
                    }
                    // get list of available commands
                    chat.getCommands()
                        .done(function (commands) {
                            ui.setCommands(commands);
                        });
                });
        });

        connection.hub.reconnected(function () {
            if (checkingStatus === true) {
                return;
            }

            checkingStatus = true;

            chat.checkStatus()
                .done(function (requiresUpdate) {
                    if (requiresUpdate === true) {
                        ui.showUpdateUI();
                    }
                })
                .always(function () {
                    checkingStatus = false;
                });
        });

        connection.hub.disconnected(function () {
            ui.showDisconnectUI();
        });

        connection.hub.error(function (err) {
            // Make all pening messages failed if there's an error
            for (var id in pendingMessages) {
                clearTimeout(pendingMessages[id]);
                ui.failMessage(id);
                delete pendingMessages[id];
            }
        });

    });

})(jQuery, $.connection, window, window.chat.ui, window.chat.utility);