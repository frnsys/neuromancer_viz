// Much credit to Jonathan Dahan (@jedahan),
// for a lot of this is directly taken from
// his pagesounds project (https://github.com/jedahan/pagesounds).

var pcap    = require('pcap'),
    session = pcap.createSession('en0', ''),
    request = require('request'),
    net     = require('net'),
    geoip   = require('geoip-lite'),
    parser  = require('http-string-parser'),

    clients = [],   // Keep track of connected clients.
    updates = [],   // Keep track of updates to send to clients.
    geos    = {},   // Keep track of geo locations for IPs.

    PORT = 3000,
    DEFAULT_GEO = { ll: [0, 0] }

// http://stackoverflow.com/a/4676258/1097920
function keypath_exists(obj, keypath) {
    var parts = keypath.split('.');
        for(var i = 0, l = parts.length; i < l; i++) {
            var part = parts[i];
            if(obj !== null && typeof obj === "object" && part in obj) {
                obj = obj[part];
            }
            else {
                return false;
            }
        }
    return true;
}

function get_local_ip() {
    var os = require('os'),
        interfaces = os.networkInterfaces();

        for (i in interfaces) {
            for (j in interfaces[i]) {
                var address = interfaces[i][j];
                if (address.family === 'IPv4' && !address.internal) {
                    return address.address;
                }
            }
        }
}

function get_remote_ip(cb) {
    var ip_service = 'http://wtfismyip.com/text';
    request(ip_service, function(err, resp, body) {
        cb(body.trim());
    });
}

var local_ip = get_local_ip(),
    remote_ip;

get_remote_ip(function(ip) {
    remote_ip = ip;
});

server = net.createServer(function(socket) {
    console.log('New client has connected from ' + socket.remoteAddress + ':' + socket.remotePort);
    clients.push(socket);

    // When the client makes a request...
    socket.on('data', function(data) {
        console.log('data request');
        var data = JSON.stringify(updates) + '\0'
        console.log(data);
        for (var i=0; i < clients.length; i++) {
            var client = clients[i]
            socket.write(data);
        }
        updates = [];
    });

    // Clean up the connection on close.
    socket.on('close', function(data) {
        clients.splice(clients.indexOf(socket), 1);
        socket.destroy();
    });

    // Clean up the connection on timeout.
    socket.on('timeout', function(data) {
        clients.splice(clients.indexOf(socket), 1);
        socket.end();
    });
});
server.listen(PORT);
console.log('Server started on port ' + PORT);


session.on('packet', function(raw) {
    var packet = pcap.decode.packet(raw);
    if (keypath_exists(packet, 'link.ip.tcp.data')) {
        var ip       = packet.link.ip,
            src      = ip.saddr,
            dest     = ip.daddr,
            src_geo  = geos[src] || geoip.lookup(src) || DEFAULT_GEO,
            dest_geo = geos[dest] || geoip.lookup(dest) || DEFAULT_GEO,
            data     = ip.tcp.data.toString(),
            resp     = parser.parseResponse(data),
            host     = src;

        if (keypath_exists(resp, 'headers.Host')) {
            host = resp.headers.Host;
        }

        // TO DO filter out local traffic by getting the
        // local network octet and checking against it.

        // If the size of the update gets too big,
        // Unity freezes...
        if (updates.length > 10) {
            updates.pop();
        }
        updates.push({
            'from': {
                'ip': src,
                'x': src_geo.ll[0],
                'y': src_geo.ll[1],
                'host': host
            },
            'to':  {
                'ip': dest,
                'x': dest_geo.ll[0],
                'y': dest_geo.ll[1]
            }
        });
    }
});