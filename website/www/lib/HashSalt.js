//CALL EXAMPLE:
//PBKDF2($('#txtPassword').val(), 'FABRIC.COM.SALT', 128 / 8, { iterations: 1000 })

var util = {
    rotl: function (n, b) {
        return (n << b) | (n >>> (32 - b));
    },
    rotr: function (n, b) {
        return (n << (32 - b)) | (n >>> b);
    },
    endian: function (n) {
        if (n.constructor == Number) {
            return util.rotl(n, 8) & 0x00FF00FF |
			       util.rotl(n, 24) & 0xFF00FF00;
        }
        for (var i = 0; i < n.length; i++)
            n[i] = util.endian(n[i]);
        return n;
    },
    randomBytes: function (n) {
        for (var bytes = []; n > 0; n--)
            bytes.push(Math.floor(Math.random() * 256));
        return bytes;
    },
    bytesToWords: function (bytes) {
        for (var words = [], i = 0, b = 0; i < bytes.length; i++, b += 8)
            words[b >>> 5] |= (bytes[i] & 0xFF) << (24 - b % 32);
        return words;
    },
    wordsToBytes: function (words) {
        for (var bytes = [], b = 0; b < words.length * 32; b += 8)
            bytes.push((words[b >>> 5] >>> (24 - b % 32)) & 0xFF);
        return bytes;
    },
    bytesToHex: function (bytes) {
        for (var hex = [], i = 0; i < bytes.length; i++) {
            hex.push((bytes[i] >>> 4).toString(16));
            hex.push((bytes[i] & 0xF).toString(16));
        }
        return hex.join("");
    },
    hexToBytes: function (hex) {
        for (var bytes = [], c = 0; c < hex.length; c += 2)
            bytes.push(parseInt(hex.substr(c, 2), 16));
        return bytes;
    },
    bytesToBase64: function (bytes) {
        if (typeof btoa == "function") return btoa(Binary.bytesToString(bytes));
        for (var base64 = [], i = 0; i < bytes.length; i += 3) {
            var triplet = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2];
            for (var j = 0; j < 4; j++) {
                if (i * 8 + j * 6 <= bytes.length * 8)
                    base64.push(base64map.charAt((triplet >>> 6 * (3 - j)) & 0x3F));
                else base64.push("=");
            }
        }
        return base64.join("");

    },
    base64ToBytes: function (base64) {
        if (typeof atob == "function") return Binary.stringToBytes(atob(base64));
        base64 = base64.replace(/[^A-Z0-9+\/]/ig, "");
        for (var bytes = [], i = 0, imod4 = 0; i < base64.length; imod4 = ++i % 4) {
            if (imod4 == 0) continue;
            bytes.push(((base64map.indexOf(base64.charAt(i - 1)) & (Math.pow(2, -2 * imod4 + 8) - 1)) << (imod4 * 2)) |
			           (base64map.indexOf(base64.charAt(i)) >>> (6 - imod4 * 2)));
        }
        return bytes;
    }
};

var charenc = {};
var Binary = charenc.Binary = {
    stringToBytes: function (str) {
        for (var bytes = [], i = 0; i < str.length; i++)
            bytes.push(str.charCodeAt(i) & 0xFF);
        return bytes;
    },
    bytesToString: function (bytes) {
        for (var str = [], i = 0; i < bytes.length; i++)
            str.push(String.fromCharCode(bytes[i]));
        return str.join("");
    }

};
var UTF8 = charenc.UTF8 = {
    stringToBytes: function (str) {
        return Binary.stringToBytes(unescape(encodeURIComponent(str)));
    },
    bytesToString: function (bytes) {
        return decodeURIComponent(escape(Binary.bytesToString(bytes)));
    }

};
var SHA1 = function (message, options) {
    var digestbytes = util.wordsToBytes(SHA1._sha1(message));
    return options && options.asBytes ? digestbytes :
	       options && options.asString ? Binary.bytesToString(digestbytes) :
	       util.bytesToHex(digestbytes);
};
SHA1._sha1 = function (message) {
    if (message.constructor == String) message = UTF8.stringToBytes(message);
    var m = util.bytesToWords(message),
	    l = message.length * 8,
	    w = [],
	    H0 = 1732584193,
	    H1 = -271733879,
	    H2 = -1732584194,
	    H3 = 271733878,
	    H4 = -1009589776;
    m[l >> 5] |= 0x80 << (24 - l % 32);
    m[((l + 64 >>> 9) << 4) + 15] = l;
    for (var i = 0; i < m.length; i += 16) {
        var a = H0,
		    b = H1,
		    c = H2,
		    d = H3,
		    e = H4;
        for (var j = 0; j < 80; j++) {

            if (j < 16) w[j] = m[i + j];
            else {
                var n = w[j - 3] ^ w[j - 8] ^ w[j - 14] ^ w[j - 16];
                w[j] = (n << 1) | (n >>> 31);
            }
            var t = ((H0 << 5) | (H0 >>> 27)) + H4 + (w[j] >>> 0) + (
			         j < 20 ? (H1 & H2 | ~H1 & H3) + 1518500249 :
			         j < 40 ? (H1 ^ H2 ^ H3) + 1859775393 :
			         j < 60 ? (H1 & H2 | H1 & H3 | H2 & H3) - 1894007588 :
			                  (H1 ^ H2 ^ H3) - 899497514);
            H4 = H3;
            H3 = H2;
            H2 = (H1 << 30) | (H1 >>> 2);
            H1 = H0;
            H0 = t;
        }
        H0 += a;
        H1 += b;
        H2 += c;
        H3 += d;
        H4 += e;
    }
    return [H0, H1, H2, H3, H4];

};

SHA1._blocksize = 16;
SHA1._digestsize = 20;

function HMAC(hasher, message, key, options) {
    if (message.constructor == String) message = UTF8.stringToBytes(message);
    if (key.constructor == String) key = UTF8.stringToBytes(key);
    if (key.length > hasher._blocksize * 4)
        key = hasher(key, { asBytes: true });
    var okey = key.slice(0),
	    ikey = key.slice(0);
    for (var i = 0; i < hasher._blocksize * 4; i++) {
        okey[i] ^= 0x5C;
        ikey[i] ^= 0x36;
    }
    var hmacbytes = hasher(okey.concat(hasher(ikey.concat(message), { asBytes: true })), { asBytes: true });
    return options && options.asBytes ? hmacbytes :
	       options && options.asString ? Binary.bytesToString(hmacbytes) :
	       util.bytesToHex(hmacbytes);

};

function PBKDF2(password, salt, keylen, options) {
    if (password.constructor == String) password = UTF8.stringToBytes(password);
    if (salt.constructor == String) salt = UTF8.stringToBytes(salt);
    var hasher = options && options.hasher || SHA1,
        iterations = options && options.iterations || 1;
    function PRF(password, salt) {
        return HMAC(hasher, salt, password, { asBytes: true });
    }
    var derivedKeyBytes = [],
        blockindex = 1;
    while (derivedKeyBytes.length < keylen) {
        var block = PRF(password, salt.concat(util.wordsToBytes([blockindex])));
        for (var u = block, i = 1; i < iterations; i++) {
            u = PRF(password, u);
            for (var j = 0; j < block.length; j++) block[j] ^= u[j];
        }
        derivedKeyBytes = derivedKeyBytes.concat(block);
        blockindex++;
    }
    derivedKeyBytes.length = keylen;
    return options && options.asBytes ? derivedKeyBytes :
           options && options.asString ? Binary.bytesToString(derivedKeyBytes) :
           util.bytesToHex(derivedKeyBytes);
};



