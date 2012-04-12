$(function () {
    var $divs = $(".graph-node");

    var node = function (str) {
        var i = str.split(':');
        return {
            hash: i[0],
            color: +i[1]
        };
    };

    var split = function (hashes) {
        var result = hashes.split(',');
        var empty;
        while ((empty = result.indexOf('')) != -1) {
            result.splice(empty, 1);
        }

        for (var i = 0; i < result.length; i++) {
            result[i] = node(result[i]);
        }

        return result;
    };

    var data = [];
    $divs.each(function () {
        data.push({
            node: node($(this).data('node')),
            parents: split($(this).data('parent-nodes')),
            incoming: split($(this).data('incoming-nodes')),
            div: this
        });
    });

    var usePalette = true;
    var palette = [
        "0061B0",
        "6A4A3D",
        "911822",
        "CCAD49",
        "439959",
        "A01E86",
        "875B0E",
        "EA4517",
        "2B14AD",
        "3E6000",
        "68727F"
    ];

    var color = function (node) {
        if (usePalette) {
            return palette[node.color % palette.length];
        } else {
            return node.hash.substr(0, 6);
        }
    };

    var find = function (list, predicate) {
        for (var i = 0; i < list.length; i++) {
            if (predicate(list[i])) {
                return i;
            }
        }

        return -1;
    };

    var maxWidth = 1;
    var shapes = [];
    for (var row = 0; row < data.length; row++) {
        var entry = data[row];
        var node = entry.node;
        var parents = entry.parents;
        var incoming = entry.incoming;
        var outgoing = row < data.length - 1 ? data[row + 1].incoming : [];

        var col = find(incoming, function (n) { return n.hash == node.hash; });
        if (col == -1) {
            col = incoming.length;
            incoming.push(node);
        }

        for (var i = 0; i < incoming.length; i++) {
            var o = find(outgoing, function (n) { return n.hash == incoming[i].hash; });
            if (o != -1) {
                shapes.push({ type: "connection", start: { x: i, y: row }, end: { x: o, y: row + 1 }, color: color(incoming[i]) });
            }
        }

        for (var p = parents.length - 1; p >= 0; p--) {
            var pCol = find(outgoing, function (n) { return n.hash == parents[p].hash; });
            if (pCol != -1) {
                shapes.push({ type: "connection", start: { x: col, y: row }, end: { x: pCol, y: row + 1 }, color: color(parents[p]) });
            }
        }

        shapes.push({ type: "circle", center: { x: col, y: row }, color: color(node) });

        maxWidth = Math.max(maxWidth, incoming.length);
    }

    var canvas = $("#graph-canvas")[0];
    var context = canvas.getContext("2d");

    var colWidth = 13;
    var rowHeight = 24;
    var lineWidth = 2;
    var curveLine = true;
    var dotRadius = 3;
    var dotBorder = 0;
    var margin = dotRadius * 2;

    canvas.width = margin * 2 + maxWidth * colWidth;
    canvas.height = margin * 2 + (data.length - 1) * rowHeight;

    // Position the text.
    for (var y = 0; y < data.length; y++) {
        $div = $(data[y].div);
        var h = $div.height();
        $div.css({
            position: "absolute",
            top: (margin + y * rowHeight - h / 2) + "px",
            left: (margin + data[y].incoming.length * colWidth) + "px"
        });
    }

    // Draw the lines and nodes.
    for (var i = 0; i < shapes.length; i++) {
        if (shapes[i].type == "connection") {
            var start = shapes[i].start;
            var end = shapes[i].end;
            var color = shapes[i].color;
            context.beginPath();
            context.moveTo(margin + start.x * colWidth, margin + start.y * rowHeight);
            if (curveLine) {
                context.bezierCurveTo(margin + start.x * colWidth, margin + end.y * rowHeight - rowHeight / 2, margin + end.x * colWidth, margin + start.y * rowHeight + rowHeight / 2, margin + end.x * colWidth, margin + end.y * rowHeight);
            } else {
                context.lineTo(margin + end.x * colWidth, margin + end.y * rowHeight);
            }
            context.strokeStyle = "#" + color;
            context.lineWidth = lineWidth;
            context.stroke();
        } else if (shapes[i].type == "circle") {
            var center = shapes[i].center;
            var color = shapes[i].color;
            context.beginPath();
            context.arc(margin + center.x * colWidth, margin + center.y * rowHeight, dotRadius, 0, 2 * Math.PI, false);
            context.fillStyle = "#" + color;
            context.fill();
            if (dotBorder) {
                context.strokeStyle = "#000";
                context.lineWidth = dotBorder;
                context.stroke();
            }
        }
    }
});