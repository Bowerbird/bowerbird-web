/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// GeospatialFormView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'licences', 'multiselect', 'async!http://maps.google.com/maps/api/js?sensor=false&region=AU'],
function ($, _, Backbone, app, ich, licences) {

    var australia = new google.maps.LatLng(-29.191427, 134.472126); // Centre on Australia

    /*
    The following code from : http://home.hiwaay.net/~taylorc/toolbox/geography/geoutm.html
    */

    var pi = 3.14159265358979;

    /* Ellipsoid model constants (actual values here are for WGS84) */
    var sm_a = 6378137.0;
    var sm_b = 6356752.314;
    var sm_EccSquared = 6.69437999013e-03;
    var UTMScaleFactor = 0.9996;

    /*
    * DegToRad
    *
    * Converts degrees to radians.
    *
    */
    var DegToRad = function (deg) {
        return (deg / 180.0 * pi);
    };

    /*
    * RadToDeg
    *
    * Converts radians to degrees.
    *
    */
    var RadToDeg = function (rad) {
        return (rad / pi * 180.0);
    };

    /*
    * ArcLengthOfMeridian
    *
    * Computes the ellipsoidal distance from the equator to a point at a
    * given latitude.
    *
    * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
    * GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
    *
    * Inputs:
    *     phi - Latitude of the point, in radians.
    *
    * Globals:
    *     sm_a - Ellipsoid model major axis.
    *     sm_b - Ellipsoid model minor axis.
    *
    * Returns:
    *     The ellipsoidal distance of the point from the equator, in meters.
    *
    */
    var ArcLengthOfMeridian = function (phi) {
        var alpha, beta, gamma, delta, epsilon, n;
        var result;

        /* Precalculate n */
        n = (sm_a - sm_b) / (sm_a + sm_b);

        /* Precalculate alpha */
        alpha = ((sm_a + sm_b) / 2.0)
            * (1.0 + (Math.pow(n, 2.0) / 4.0) + (Math.pow(n, 4.0) / 64.0));

        /* Precalculate beta */
        beta = (-3.0 * n / 2.0) + (9.0 * Math.pow(n, 3.0) / 16.0)
            + (-3.0 * Math.pow(n, 5.0) / 32.0);

        /* Precalculate gamma */
        gamma = (15.0 * Math.pow(n, 2.0) / 16.0)
            + (-15.0 * Math.pow(n, 4.0) / 32.0);

        /* Precalculate delta */
        delta = (-35.0 * Math.pow(n, 3.0) / 48.0)
            + (105.0 * Math.pow(n, 5.0) / 256.0);

        /* Precalculate epsilon */
        epsilon = (315.0 * Math.pow(n, 4.0) / 512.0);

        /* Now calculate the sum of the series and return */
        result = alpha
            * (phi + (beta * Math.sin(2.0 * phi))
                + (gamma * Math.sin(4.0 * phi))
                + (delta * Math.sin(6.0 * phi))
                + (epsilon * Math.sin(8.0 * phi)));

        return result;
    };

    /*
    * UTMCentralMeridian
    *
    * Determines the central meridian for the given UTM zone.
    *
    * Inputs:
    *     zone - An integer value designating the UTM zone, range [1,60].
    *
    * Returns:
    *   The central meridian for the given UTM zone, in radians, or zero
    *   if the UTM zone parameter is outside the range [1,60].
    *   Range of the central meridian is the radian equivalent of [-177,+177].
    *
    */
    var UTMCentralMeridian = function (zone) {
        var cmeridian;

        cmeridian = DegToRad(-183.0 + (zone * 6.0));

        return cmeridian;
    };

    /*
    * FootpointLatitude
    *
    * Computes the footpoint latitude for use in converting transverse
    * Mercator coordinates to ellipsoidal coordinates.
    *
    * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
    *   GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
    *
    * Inputs:
    *   y - The UTM northing coordinate, in meters.
    *
    * Returns:
    *   The footpoint latitude, in radians.
    *
    */
    var FootpointLatitude = function (y) {
        var y_, alpha_, beta_, gamma_, delta_, epsilon_, n;
        var result;

        /* Precalculate n (Eq. 10.18) */
        n = (sm_a - sm_b) / (sm_a + sm_b);

        /* Precalculate alpha_ (Eq. 10.22) */
        /* (Same as alpha in Eq. 10.17) */
        alpha_ = ((sm_a + sm_b) / 2.0)
            * (1 + (Math.pow(n, 2.0) / 4) + (Math.pow(n, 4.0) / 64));

        /* Precalculate y_ (Eq. 10.23) */
        y_ = y / alpha_;

        /* Precalculate beta_ (Eq. 10.22) */
        beta_ = (3.0 * n / 2.0) + (-27.0 * Math.pow(n, 3.0) / 32.0)
            + (269.0 * Math.pow(n, 5.0) / 512.0);

        /* Precalculate gamma_ (Eq. 10.22) */
        gamma_ = (21.0 * Math.pow(n, 2.0) / 16.0)
            + (-55.0 * Math.pow(n, 4.0) / 32.0);

        /* Precalculate delta_ (Eq. 10.22) */
        delta_ = (151.0 * Math.pow(n, 3.0) / 96.0)
            + (-417.0 * Math.pow(n, 5.0) / 128.0);

        /* Precalculate epsilon_ (Eq. 10.22) */
        epsilon_ = (1097.0 * Math.pow(n, 4.0) / 512.0);

        /* Now calculate the sum of the series (Eq. 10.21) */
        result = y_ + (beta_ * Math.sin(2.0 * y_))
            + (gamma_ * Math.sin(4.0 * y_))
            + (delta_ * Math.sin(6.0 * y_))
            + (epsilon_ * Math.sin(8.0 * y_));

        return result;
    };

    /*
    * MapLatLonToXY
    *
    * Converts a latitude/longitude pair to x and y coordinates in the
    * Transverse Mercator projection.  Note that Transverse Mercator is not
    * the same as UTM; a scale factor is required to convert between them.
    *
    * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
    * GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
    *
    * Inputs:
    *    phi - Latitude of the point, in radians.
    *    lambda - Longitude of the point, in radians.
    *    lambda0 - Longitude of the central meridian to be used, in radians.
    *
    * Outputs:
    *    xy - A 2-element array containing the x and y coordinates
    *         of the computed point.
    *
    * Returns:
    *    The function does not return a value.
    *
    */
    var MapLatLonToXY = function (phi, lambda, lambda0, xy) {
        var N, nu2, ep2, t, t2, l;
        var l3coef, l4coef, l5coef, l6coef, l7coef, l8coef;
        var tmp;

        /* Precalculate ep2 */
        ep2 = (Math.pow(sm_a, 2.0) - Math.pow(sm_b, 2.0)) / Math.pow(sm_b, 2.0);

        /* Precalculate nu2 */
        nu2 = ep2 * Math.pow(Math.cos(phi), 2.0);

        /* Precalculate N */
        N = Math.pow(sm_a, 2.0) / (sm_b * Math.sqrt(1 + nu2));

        /* Precalculate t */
        t = Math.tan(phi);
        t2 = t * t;
        tmp = (t2 * t2 * t2) - Math.pow(t, 6.0);

        /* Precalculate l */
        l = lambda - lambda0;

        /* Precalculate coefficients for l**n in the equations below
        so a normal human being can read the expressions for easting
        and northing
        -- l**1 and l**2 have coefficients of 1.0 */
        l3coef = 1.0 - t2 + nu2;

        l4coef = 5.0 - t2 + 9 * nu2 + 4.0 * (nu2 * nu2);

        l5coef = 5.0 - 18.0 * t2 + (t2 * t2) + 14.0 * nu2
            - 58.0 * t2 * nu2;

        l6coef = 61.0 - 58.0 * t2 + (t2 * t2) + 270.0 * nu2
            - 330.0 * t2 * nu2;

        l7coef = 61.0 - 479.0 * t2 + 179.0 * (t2 * t2) - (t2 * t2 * t2);

        l8coef = 1385.0 - 3111.0 * t2 + 543.0 * (t2 * t2) - (t2 * t2 * t2);

        /* Calculate easting (x) */
        xy[0] = N * Math.cos(phi) * l
            + (N / 6.0 * Math.pow(Math.cos(phi), 3.0) * l3coef * Math.pow(l, 3.0))
            + (N / 120.0 * Math.pow(Math.cos(phi), 5.0) * l5coef * Math.pow(l, 5.0))
            + (N / 5040.0 * Math.pow(Math.cos(phi), 7.0) * l7coef * Math.pow(l, 7.0));

        /* Calculate northing (y) */
        xy[1] = ArcLengthOfMeridian(phi)
            + (t / 2.0 * N * Math.pow(Math.cos(phi), 2.0) * Math.pow(l, 2.0))
            + (t / 24.0 * N * Math.pow(Math.cos(phi), 4.0) * l4coef * Math.pow(l, 4.0))
            + (t / 720.0 * N * Math.pow(Math.cos(phi), 6.0) * l6coef * Math.pow(l, 6.0))
            + (t / 40320.0 * N * Math.pow(Math.cos(phi), 8.0) * l8coef * Math.pow(l, 8.0));

        return;
    };

    /*
    * MapXYToLatLon
    *
    * Converts x and y coordinates in the Transverse Mercator projection to
    * a latitude/longitude pair.  Note that Transverse Mercator is not
    * the same as UTM; a scale factor is required to convert between them.
    *
    * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
    *   GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
    *
    * Inputs:
    *   x - The easting of the point, in meters.
    *   y - The northing of the point, in meters.
    *   lambda0 - Longitude of the central meridian to be used, in radians.
    *
    * Outputs:
    *   philambda - A 2-element containing the latitude and longitude
    *               in radians.
    *
    * Returns:
    *   The function does not return a value.
    *
    * Remarks:
    *   The local variables Nf, nuf2, tf, and tf2 serve the same purpose as
    *   N, nu2, t, and t2 in MapLatLonToXY, but they are computed with respect
    *   to the footpoint latitude phif.
    *
    *   x1frac, x2frac, x2poly, x3poly, etc. are to enhance readability and
    *   to optimize computations.
    *
    */
    var MapXYToLatLon = function (x, y, lambda0, philambda) {
        var phif, Nf, Nfpow, nuf2, ep2, tf, tf2, tf4, cf;
        var x1frac, x2frac, x3frac, x4frac, x5frac, x6frac, x7frac, x8frac;
        var x2poly, x3poly, x4poly, x5poly, x6poly, x7poly, x8poly;

        /* Get the value of phif, the footpoint latitude. */
        phif = FootpointLatitude(y);

        /* Precalculate ep2 */
        ep2 = (Math.pow(sm_a, 2.0) - Math.pow(sm_b, 2.0))
            / Math.pow(sm_b, 2.0);

        /* Precalculate cos (phif) */
        cf = Math.cos(phif);

        /* Precalculate nuf2 */
        nuf2 = ep2 * Math.pow(cf, 2.0);

        /* Precalculate Nf and initialize Nfpow */
        Nf = Math.pow(sm_a, 2.0) / (sm_b * Math.sqrt(1 + nuf2));
        Nfpow = Nf;

        /* Precalculate tf */
        tf = Math.tan(phif);
        tf2 = tf * tf;
        tf4 = tf2 * tf2;

        /* Precalculate fractional coefficients for x**n in the equations
        below to simplify the expressions for latitude and longitude. */
        x1frac = 1.0 / (Nfpow * cf);

        Nfpow *= Nf; /* now equals Nf**2) */
        x2frac = tf / (2.0 * Nfpow);

        Nfpow *= Nf; /* now equals Nf**3) */
        x3frac = 1.0 / (6.0 * Nfpow * cf);

        Nfpow *= Nf; /* now equals Nf**4) */
        x4frac = tf / (24.0 * Nfpow);

        Nfpow *= Nf; /* now equals Nf**5) */
        x5frac = 1.0 / (120.0 * Nfpow * cf);

        Nfpow *= Nf; /* now equals Nf**6) */
        x6frac = tf / (720.0 * Nfpow);

        Nfpow *= Nf; /* now equals Nf**7) */
        x7frac = 1.0 / (5040.0 * Nfpow * cf);

        Nfpow *= Nf; /* now equals Nf**8) */
        x8frac = tf / (40320.0 * Nfpow);

        /* Precalculate polynomial coefficients for x**n.
        -- x**1 does not have a polynomial coefficient. */
        x2poly = -1.0 - nuf2;

        x3poly = -1.0 - 2 * tf2 - nuf2;

        x4poly = 5.0 + 3.0 * tf2 + 6.0 * nuf2 - 6.0 * tf2 * nuf2
            - 3.0 * (nuf2 * nuf2) - 9.0 * tf2 * (nuf2 * nuf2);

        x5poly = 5.0 + 28.0 * tf2 + 24.0 * tf4 + 6.0 * nuf2 + 8.0 * tf2 * nuf2;

        x6poly = -61.0 - 90.0 * tf2 - 45.0 * tf4 - 107.0 * nuf2
            + 162.0 * tf2 * nuf2;

        x7poly = -61.0 - 662.0 * tf2 - 1320.0 * tf4 - 720.0 * (tf4 * tf2);

        x8poly = 1385.0 + 3633.0 * tf2 + 4095.0 * tf4 + 1575 * (tf4 * tf2);

        /* Calculate latitude */
        philambda[0] = phif + x2frac * x2poly * (x * x)
            + x4frac * x4poly * Math.pow(x, 4.0)
            + x6frac * x6poly * Math.pow(x, 6.0)
            + x8frac * x8poly * Math.pow(x, 8.0);

        /* Calculate longitude */
        philambda[1] = lambda0 + x1frac * x
            + x3frac * x3poly * Math.pow(x, 3.0)
            + x5frac * x5poly * Math.pow(x, 5.0)
            + x7frac * x7poly * Math.pow(x, 7.0);

        return;
    };

    /*
    * LatLonToUTMXY
    *
    * Converts a latitude/longitude pair to x and y coordinates in the
    * Universal Transverse Mercator projection.
    *
    * Inputs:
    *   lat - Latitude of the point, in radians.
    *   lon - Longitude of the point, in radians.
    *   zone - UTM zone to be used for calculating values for x and y.
    *          If zone is less than 1 or greater than 60, the routine
    *          will determine the appropriate zone from the value of lon.
    *
    * Outputs:
    *   xy - A 2-element array where the UTM x and y values will be stored.
    *
    * Returns:
    *   The UTM zone used for calculating the values of x and y.
    *
    */
    var LatLonToUTMXY = function (lat, lon, zone, xy) {
        MapLatLonToXY(lat, lon, UTMCentralMeridian(zone), xy);

        /* Adjust easting and northing for UTM system. */
        xy[0] = xy[0] * UTMScaleFactor + 500000.0;
        xy[1] = xy[1] * UTMScaleFactor;
        if (xy[1] < 0.0)
            xy[1] = xy[1] + 10000000.0;

        return zone;
    };

    /*
    * UTMXYToLatLon
    *
    * Converts x and y coordinates in the Universal Transverse Mercator
    * projection to a latitude/longitude pair.
    *
    * Inputs:
    *	x - The easting of the point, in meters.
    *	y - The northing of the point, in meters.
    *	zone - The UTM zone in which the point lies.
    *	southhemi - True if the point is in the southern hemisphere;
    *               false otherwise.
    *
    * Outputs:
    *	latlon - A 2-element array containing the latitude and
    *            longitude of the point, in radians.
    *
    * Returns:
    *	The function does not return a value.
    *
    */
    var UTMXYToLatLon = function (x, y, zone, southhemi, latlon) {
        var cmeridian;

        x -= 500000.0;
        x /= UTMScaleFactor;

        /* If in southern hemisphere, adjust y accordingly. */
        if (southhemi)
            y -= 10000000.0;

        y /= UTMScaleFactor;

        cmeridian = UTMCentralMeridian(zone);
        MapXYToLatLon(x, y, cmeridian, latlon);

        return;
    };

    /*
    DMS
    */
    function DmsCalc() {
    };

    DmsCalc.NORTH = 'N';
    DmsCalc.SOUTH = 'S';
    DmsCalc.EAST = 'E';
    DmsCalc.WEST = 'W';

    DmsCalc.roundToDecimal = function (inputNum, numPoints) {
        var multiplier = Math.pow(10, numPoints);
        return Math.round(inputNum * multiplier) / multiplier;
    };

    DmsCalc.decimalToDMS = function (location, hemisphere) {
        if (location < 0) location *= -1; // strip dash '-'

        var degrees = Math.floor(location);          // strip decimal remainer for degrees
        var minutesFromRemainder = (location - degrees) * 60;       // multiply the remainer by 60
        var minutes = Math.floor(minutesFromRemainder);       // get minutes from integer
        var secondsFromRemainder = (minutesFromRemainder - minutes) * 60;   // multiply the remainer by 60
        var seconds = DmsCalc.roundToDecimal(secondsFromRemainder, 2); // get minutes by rounding to integer

        return {
            Degrees: degrees,
            Minutes: minutes,
            Seconds: seconds,
            Direction: hemisphere
        };
    };

    DmsCalc.decimalLatToDMS = function (location) {
        var hemisphere = (location < 0) ? DmsCalc.SOUTH : DmsCalc.NORTH; // south if negative
        return DmsCalc.decimalToDMS(location, hemisphere);
    };

    DmsCalc.decimalLongToDMS = function (location) {
        var hemisphere = (location < 0) ? DmsCalc.WEST : DmsCalc.EAST;  // west if negative
        return DmsCalc.decimalToDMS(location, hemisphere);
    };

    DmsCalc.DMSToDecimal = function (degrees, minutes, seconds, hemisphere) {
        var ddVal = degrees + minutes / 60 + seconds / 3600;
        ddVal = (hemisphere == DmsCalc.SOUTH || hemisphere == DmsCalc.WEST) ? ddVal * -1 : ddVal;
        return DmsCalc.roundToDecimal(ddVal, 5);
    };

    var LatLongDecimalCoordinates = Backbone.Model.extend({
        defaults: {
            Latitude: null,
            Longitude: null
        },

        initialize: function () {

        },

        setCoords: function (lat, lng) {
            var errors = [];
            var isValid = true;

            if (lat.length == 0 || 
                (lat.length > 0 && isNaN(parseFloat(lat))) ||
                parseFloat(lat) < -90.0 || 
                90.0 < parseFloat(lat)) {
                errors.push({ Field: 'Latitude', Message: 'Please enter a valid latitude value. Latitude values must be between -90 and 90.' });
                isValid = false;
            }

            if (lng.length == 0 || 
                (lng.length > 0 && isNaN(parseFloat(lng))) ||
                parseFloat(lng) < -180.0 || 
                180.0 <= parseFloat(lng)) {
                errors.push({ Field: 'Longitude', Message: 'Please enter a valid longitude value. Longitude values must be between -180 and 180.' });
                isValid = false;
            }

            if (isValid) {
                this.set('Latitude', parseFloat(lat));
                this.set('Longitude', parseFloat(lng));
            } else {
                this.set('Latitude', null);
                this.set('Longitude', null);
            }

            this.trigger('validated', this, errors);

            return;
        },

        getDecimalLatLong: function () {
            return this.attributes;
        },

        isValid: function () {
            return this.get('Latitude') != null && this.get('Longitude') != null;
        }
    });

    var UtmCoordinates = Backbone.Model.extend({
        defaults: {
            Easting: null,
            Northing: null,
            Zone: null,
            Hemisphere: 'South'
        },

        initialize: function (data, options) {
            if (options) {
                this.attributes = this.convertLatLongDecimalToUtm(options.Latitude, options.Longitude);
            }
        },

        setCoords: function (easting, northing, zn, hem) {
            var errors = [];
            var isValid = true;

            if (easting.length == 0 ||
                (easting.length > 0 && isNaN(parseFloat(easting)))) {
                errors.push({ Field: 'Easting', Message: 'Please enter a valid easting value' });
                isValid = false;
            }

            if (northing.length == 0 ||
                (northing.length > 0 && isNaN(parseFloat(northing)))) {
                errors.push({ Field: 'Northing', Message: 'Please enter a valid northing value' });
                isValid = false;
            }

            if (zn.length == 0 ||
                (zn.length > 0 && isNaN(parseInt(zn))) ||
                parseFloat(zn) < 1 || 
                60 < parseFloat(zn)) {
                errors.push({ Field: 'Zone', Message: 'Please enter a valid zone value. Zone values must be between 1 and 60.' });
                isValid = false;
            }

            if (isValid) {
                this.set('Easting', parseFloat(easting));
                this.set('Northing', parseFloat(northing));
                this.set('Zone', parseFloat(zn));
                this.set('Hemisphere', hem);
            } else {
                this.set('Easting', null);
                this.set('Northing', null);
                this.set('Zone', null);
                this.set('Hemisphere', hem);
            }

            this.trigger('validated', this, errors);

            return;
        },

        getDecimalLatLong: function () {
            return this.convertUtmToLatLongDecimal(
                this.get('Easting'),
                this.get('Northing'),
                this.get('Zone'),
                this.get('Hemisphere'));
        },

        convertLatLongDecimalToUtm: function (lat, lng) {
            // Compute the UTM zone
            var xy = new Array(2);
            var zone = Math.floor((lng + 180.0) / 6) + 1;
            zone = LatLonToUTMXY(DegToRad(lat), DegToRad(lng), zone, xy);

            var hemisphere = '';
            if (lat < 0) {
                hemisphere = 'South';
            } else {
                hemisphere = 'North';
            }

            return {
                Easting: xy[0],
                Northing: xy[1],
                Zone: zone,
                Hemisphere: hemisphere
            };
        },

        convertUtmToLatLongDecimal: function (easting, northing, zn, hem) {
            var xy = new Array(2);
            UTMXYToLatLon(easting, northing, zn, hem === 'South', xy);

            return {
                Latitude: RadToDeg(xy[0]),
                Longitude: RadToDeg(xy[1])
            };
        },

        isValid: function () {
            return this.get('Easting') != null && this.get('Northing') != null && this.get('Zone') != null;
        }
    });

    var LatLongDmsCoordinates = Backbone.Model.extend({
        defaults: {
            Latitude: {
                Degrees: null,
                Minutes: null,
                Seconds: null,
                Direction: 'S'
            },
            Longitude: {
                Degrees: null,
                Minutes: null,
                Seconds: null,
                Direction: 'E'
            }
        },

        initialize: function (data, options) {
            if (options) {
                this.attributes = this.convertLatLongDecimalToDms(options.Latitude, options.Longitude);
            }
        },

        setCoords: function (latDeg, latMin, latSec, latDirection, lngDeg, lngMin, lngSec, lngDirection) {
            var errors = [];
            var isValid = true;

            if (latDeg.length == 0 || 
                (latDeg.length > 0 && isNaN(parseFloat(latDeg)))) {
                errors.push({ Field: 'LatitudeDegrees', Message: 'Please enter a valid latitude degrees value' });
                isValid = false;
            }

            if (latMin.length == 0 || 
                (latMin.length > 0 && isNaN(parseFloat(latMin)))) {
                errors.push({ Field: 'LatitudeMinutes', Message: 'Please enter a valid latitude minutes value' });
                isValid = false;
            }

            if (latSec.length == 0 || 
                (latSec.length > 0 && isNaN(parseInt(latSec)))) {
                errors.push({ Field: 'LatitudeSeconds', Message: 'Please enter a valid latitude seconds value' });
                isValid = false;
            }

            if (lngDeg.length == 0 || 
                (lngDeg.length > 0 && isNaN(parseFloat(lngDeg)))) {
                errors.push({ Field: 'LongitudeDegrees', Message: 'Please enter a valid longitude degrees value' });
                isValid = false;
            }

            if (lngMin.length == 0 || 
                (lngMin.length > 0 && isNaN(parseFloat(lngMin)))) {
                errors.push({ Field: 'LongitudeMinutes', Message: 'Please enter a valid longitude minutes value' });
                isValid = false;
            }

            if (lngSec.length == 0 || 
                (lngSec.length > 0 && isNaN(parseInt(lngSec)))) {
                errors.push({ Field: 'LongitudeSeconds', Message: 'Please enter a valid longitude seconds value' });
                isValid = false;
            }

            if (isValid) {
                this.set('Latitude', {
                    Degrees: parseFloat(latDeg),
                    Minutes: parseFloat(latMin),
                    Seconds: parseFloat(latSec),
                    Direction: latDirection
                });
                this.set('Longitude', {
                    Degrees: parseFloat(lngDeg),
                    Minutes: parseFloat(lngMin),
                    Seconds: parseFloat(lngSec),
                    Direction: lngDirection
                });
            } else {
                this.set('Latitude', {
                    Degrees: null,
                    Minutes: null,
                    Seconds: null,
                    Direction: 'S'
                });
                this.set('Longitude', {
                    Degrees: null,
                    Minutes: null,
                    Seconds: null,
                    Direction: 'E'
                });
            }

            this.trigger('validated', this, errors);

            return;
        },

        getDecimalLatLong: function () {
            return this.convertDmsToLatLongDecimal(
                this.get('Latitude').Degrees,
                this.get('Latitude').Minutes,
                this.get('Latitude').Seconds,
                this.get('Latitude').Direction,
                this.get('Longitude').Degrees,
                this.get('Longitude').Minutes,
                this.get('Longitude').Seconds,
                this.get('Longitude').Direction);
        },

        convertLatLongDecimalToDms: function (lat, lng) {
            return {
                Latitude: DmsCalc.decimalLatToDMS(lat),
                Longitude: DmsCalc.decimalLongToDMS(lng)
            };
        },

        convertDmsToLatLongDecimal: function (latDeg, latMin, latSec, latDir, lngDeg, lngMin, lngSec, lngDir) {
            return {
                Latitude: DmsCalc.DMSToDecimal(latDeg, latMin, latSec, latDir),
                Longitude: DmsCalc.DMSToDecimal(lngDeg, lngMin, lngSec, lngDir)
            };
        },

        isValid: function () {
            return this.get('Latitude').Degrees != null;
        }
    });
    
    var GeospatialFormView = Backbone.Marionette.ItemView.extend({
        id: 'edit-geospatial-form',

        template: 'GeospatialForm',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .done-button': '_done',
            'click .latlng-decimal-button': 'showLatLngDecimalForm',
            'click .latlng-dms-button': 'showLatLngDmsForm',
            'click .utm-button': 'showUtmForm',
            'click #check-utm-button': 'checkUtm',
            'click #check-latlng-decimal-button': 'checkLatLongDecimal',
            'click #check-latlng-dms-button': 'checkLatLongDms'
        },

        serializeData: function () {
            return {
                Model: {
                    Coordinates: {
                        Decimal: this.latLongDecimalCoordinates.toJSON(),
                        Dms: this.latLongDmsCoordinates.toJSON(),
                        Utm: this.utmCoordinates.toJSON()
                    },
                    HemisphereSelectList: {
                        IsNorth: this.utmCoordinates.get('Hemisphere') === 'North',
                        IsSouth: this.utmCoordinates.get('Hemisphere') === 'South'
                    },
                    LatitudeDmsDirection: {
                        IsNorth: this.latLongDmsCoordinates.get('Latitude').Direction === 'N',
                        IsSouth: this.latLongDmsCoordinates.get('Latitude').Direction === 'S'
                    },
                    LongitudeDmsDirection: {
                        IsEast: this.latLongDmsCoordinates.get('Longitude').Direction === 'E',
                        IsWest: this.latLongDmsCoordinates.get('Longitude').Direction === 'W'
                    }
                }
            }; 
        },

        initialize: function (options) {
            _.bindAll(this, 'onUtmValidation');

            this.validCoordinates = null;

            if (this.model.hasLatLong()) {
                this.validCoordinates = { Latitude: parseFloat(this.model.get('Latitude')), Longitude: parseFloat(this.model.get('Longitude')) };

                this.latLongDecimalCoordinates = new LatLongDecimalCoordinates(this.validCoordinates);
                this.latLongDmsCoordinates = new LatLongDmsCoordinates({}, this.validCoordinates);
                this.utmCoordinates = new UtmCoordinates({}, this.validCoordinates);
            } else {
                this.latLongDecimalCoordinates = new LatLongDecimalCoordinates();
                this.latLongDmsCoordinates = new LatLongDmsCoordinates();
                this.utmCoordinates = new UtmCoordinates();
            }

            this.latLongDecimalCoordinates.on('validated', this.onLatLongDecimalValidation, this);
            this.latLongDmsCoordinates.on('validated', this.onLatLongDmsValidation, this);
            this.utmCoordinates.on('validated', this.onUtmValidation, this);
        },

        checkLatLongDecimal: function (e) {
            e.preventDefault();

            this.latLongDecimalCoordinates.setCoords(
                this.$el.find('#LatitudeDecimal').val(),
                this.$el.find('#LongitudeDecimal').val());

            return false;
        },

        checkLatLongDms: function (e) {
            e.preventDefault();

            this.latLongDmsCoordinates.setCoords(
                this.$el.find('#LatitudeDmsDegrees').val(),
                this.$el.find('#LatitudeDmsMinutes').val(),
                this.$el.find('#LatitudeDmsSeconds').val(),
                this.$el.find('#LatitudeDmsDirection').val(),
                this.$el.find('#LongitudeDmsDegrees').val(),
                this.$el.find('#LongitudeDmsMinutes').val(),
                this.$el.find('#LongitudeDmsSeconds').val(),
                this.$el.find('#LongitudeDmsDirection').val()
            );

            return false;
        },

        checkUtm: function (e) {
            e.preventDefault();

            this.utmCoordinates.setCoords(
                this.$el.find('#UtmEasting').val(),
                this.$el.find('#UtmNorthing').val(),
                this.$el.find('#UtmZone').val(),
                this.$el.find('#UtmHemisphere').val());

            return false;
        },

        onLatLongDecimalValidation: function (coords, errors) {
            this.onValidation(coords, errors);

            this.$el.find('#LatitudeDecimal, #LongitudeDecimal').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'Latitude'; })) {
                this.$el.find('#LatitudeDecimal').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Longitude'; })) {
                this.$el.find('#LongitudeDecimal').addClass('input-validation-error');
            }
        },

        onLatLongDmsValidation: function (coords, errors) {
            this.onValidation(coords, errors);

            this.$el.find('#LatitudeDmsDegrees, #LatitudeDmsMinutes, #LatitudeDmsSeconds, #LongitudeDmsDegrees, #LongitudeDmsMinutes, #LongitudeDmsSeconds').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'LatitudeDegrees'; })) {
                this.$el.find('#LatitudeDmsDegrees').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'LatitudeMinutes'; })) {
                this.$el.find('#LatitudeDmsMinutes').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'LatitudeSeconds'; })) {
                this.$el.find('#LatitudeDmsSeconds').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'LongitudeDegrees'; })) {
                this.$el.find('#LongitudeDmsDegrees').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'LongitudeMinutes'; })) {
                this.$el.find('#LongitudeDmsMinutes').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'LongitudeSeconds'; })) {
                this.$el.find('#LongitudeDmsSeconds').addClass('input-validation-error');
            }
        },

        onUtmValidation: function (coords, errors) {
            this.onValidation(coords, errors);

            this.$el.find('#UtmEasting, #UtmNorthing, #UtmZone').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'Easting'; })) {
                this.$el.find('#UtmEasting').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Northing'; })) {
                this.$el.find('#UtmNorthing').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Zone'; })) {
                this.$el.find('#UtmZone').addClass('input-validation-error');
            }
        },

        onValidation: function (coords, errors) {
            if (errors.length == 0) {
                this.$el.find('.validation-summary').slideUp(function () { $(this).remove(); });
            }

            if (errors.length > 0) {
                if (this.$el.find('.validation-summary').length == 0) {
                    this.$el.find('.edit-geospatial-form').prepend(ich.ValidationSummary({
                        SummaryMessage: 'Please correct the following before continuing:',
                        Errors: errors
                    }));
                    this.$el.find('.validation-summary').slideDown();
                } else {
                    var that = this;
                    // Remove items
                    this.$el.find('.validation-summary li').each(function () {
                        var $li = that.$el.find(this);
                        var found = _.find(errors, function (err) {
                            return 'validation-field-' + err.Field === $li.attr('class');
                        });
                        if (!found) {
                            $li.slideUp(function () { $(this).remove(); });
                        }
                    });

                    // Add items
                    _.each(errors, function (err) {
                        if (this.$el.find('.validation-field-' + err.Field).length === 0) {
                            var li = $('<li class="validation-field-' + err.Field + '">' + err.Message + '</li>').css({ display: 'none' });
                            this.$el.find('.validation-summary ul').append(li);
                            li.slideDown();
                        }
                    }, this);
                }
            }

            if (errors.length == 0) {
                var latLongDec = coords.getDecimalLatLong();
                this.$el.find('#geospatial-lat-long').text(latLongDec.Latitude + ', ' + latLongDec.Longitude);
                this.$el.find('.done-button').removeAttr('disabled');

                this.showPointOnMap(new google.maps.LatLng(latLongDec.Latitude, latLongDec.Longitude));
                
                this.validCoordinates = latLongDec;
            } else {
                this.$el.find('#geospatial-lat-long').text('None');
                this.$el.find('.done-button').attr('disabled', 'disabled');

                if (this.mapMarker) {
                    this.mapMarker.setMap(null);
                    this.mapMarker = null;
                }
                this.map.panTo(australia);
                this.map.setZoom(3);

                this.validCoordinates = null;
            }
        },

        onRender: function () {
            var currentLatLng = this.latLongDecimalCoordinates.isValid() ? new google.maps.LatLng(this.latLongDecimalCoordinates.get('Latitude'), this.latLongDecimalCoordinates.get('Longitude')) : australia;
            var zoomLevel = this.latLongDecimalCoordinates.isValid() ? 7 : 3;

            var mapOptions = {
                zoom: zoomLevel,
                center: currentLatLng,
                disableDefaultUI: true,
                scrollwheel: false,
                disableDoubleClickZoom: false,
                draggable: false,
                keyboardShortcuts: false,
                mapTypeId: google.maps.MapTypeId.TERRAIN
            };

            var map = new google.maps.Map(this.$el.find('.geospatial-map').get(0), mapOptions);
            this.map = map;

            if (this.latLongDecimalCoordinates.isValid()) {
                this.$el.find('#geospatial-lat-long').text(this.latLongDecimalCoordinates.get('Latitude') + ', ' + this.latLongDecimalCoordinates.get('Longitude'));

                this.showPointOnMap(currentLatLng);

                this.$el.find('.done-button').removeAttr('disabled');
            } else {
                this.map.panTo(australia);
                this.map.setZoom(3);

                this.$el.find('.done-button').attr('disabled', 'disabled');
            }

            return this;
        },

        showPointOnMap: function (latLng) {
            // Remove map marker first
            if (this.mapMarker) {
                this.mapMarker.setMap(null);
                this.mapMarker = null;
            }

            var point = latLng;
            this.point = point;

            var image = new google.maps.MarkerImage('/img/map-pin.png',
                new google.maps.Size(43, 38),
                new google.maps.Point(0, 0)
            );

            var shadow = new google.maps.MarkerImage('/img/map-pin-shadow.png',
                new google.maps.Size(59, 32),
                new google.maps.Point(0, 0),
                new google.maps.Point(17, 32)
            );

            this.mapMarker = new google.maps.Marker({
                position: point,
                map: this.map,
                clickable: false,
                draggable: false,
                icon: image,
                shadow: shadow
            });

            var position = this.mapMarker.getPosition();
            var newPoint = new google.maps.LatLng(position.lat() + .02, position.lng());
            this.map.panTo(newPoint);
            this.map.setZoom(7);
        },

        showLatLngDecimalForm: function (e) {
            this.showGeoForm(e, '.location-latlngdecimal', '.latlng-decimal-button');
        },

        showLatLngDmsForm: function (e) {
            this.showGeoForm(e, '.location-latlngdms', '.latlng-dms-button');
        },

        showUtmForm: function (e) {
            this.showGeoForm(e, '.location-utm', '.utm-button');
        },

        showGeoForm: function (e, form, button) {
            e.preventDefault();
            this.$el.find('.latlng-decimal-button, .latlng-dms-button, .utm-button').removeClass('selected');
            this.$el.find(button).addClass('selected');
            this.$el.find('.location-latlngdecimal, .location-latlngdms, .location-utm').hide();
            this.$el.find(form).show();
            return false;
        },

        _cancel: function () {
            this.remove();
        },

        _done: function () {
            this.model.set('Latitude', this.validCoordinates.Latitude);
            this.model.set('Longitude', this.validCoordinates.Longitude);

            this.trigger('coords-done', this.model);

            this.remove();
        }
    });

    return GeospatialFormView;
});