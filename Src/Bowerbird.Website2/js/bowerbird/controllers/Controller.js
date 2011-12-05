Bowerbird.Controller = function () {
    this.eventCallbacks = [];
}

Bowerbird.Controller.prototype.constructor = Bowerbird.Controller;

Bowerbird.Controller.prototype.addEventCallback = function (eventName, listener, callback) {
    var eventCallback = {
        eventName: eventName,
        listener: listener,
        callback: callback
    };

    this.eventCallbacks.push(eventCallback);
}

Bowerbird.Controller.prototype.removeEventCallback = function (eventName, listener) {
    for (var i = 0; i < this.eventCallbacks.length; i++) {
        if (this.eventCallbacks[i].eventName == eventName && this.eventCallbacks[i].listener == listener) {
            this.eventCallbacks.splice(i, 1);
            return;
        }
    }
}

Bowerbird.Controller.prototype.notifyEventCallbacks = function (event) {
    console.log('firing event: ' + event.name);
    for (var i = 0; i < this.eventCallbacks.length; i++) {
        if (this.eventCallbacks[i].eventName == event.name) {
            setTimeout(function (callback, eventObj) { callback(eventObj) }, 1, this.eventCallbacks[i].callback, event);
        }
    }
}

// In subclasses, this method can be overriden to unbind jQuery events, etc.
Bowerbird.Controller.prototype.removeAllEventCallbacks = function () {
    while (this.eventCallbacks.length > 0) {
        this.eventCallbacks.pop();
    }
}
