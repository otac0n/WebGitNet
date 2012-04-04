$(function () {
    var $divs = $(".graph-node");

    var split = function (hashes) {
        var result = hashes.split(',');
        var empty;
        while ((empty = result.indexOf('')) != -1) {
            result.splice(empty, 1);
        }

        return result;
    };

    var data = [];
    $divs.each(function () {
        data.push({
            hash: $(this).data('hash'),
            parents: split($(this).data('parent-hashes')),
            incoming: split($(this).data('incoming-hashes'))
        });
    });

    var maxWidth = 1;
    var shapes = [];
    for (var row = 0; row < data.length; row++) {
        var entry = data[row];
        var hash = entry.hash;
        var parents = entry.parents;
        var incoming = entry.incoming;
        var outgoing = row < data.length - 1 ? data[row + 1].incoming : [];

        var col = incoming.indexOf(hash);
        if (col == -1) {
            col = incoming.length;
            incoming.push(hash);
        }

        for (var i = 0; i < incoming.length; i++) {
            var o = outgoing.indexOf(incoming[i]);
            if (o != -1) {
                shapes.push({ type: "connection", start: { x: i, y: row }, end: { x: o, y: row + 1 }, color: incoming[i].substr(0, 6) });
            }
        }

        for (var p = parents.length - 1; p >= 0; p--) {
            var pCol = outgoing.indexOf(parents[p]);
            shapes.push({ type: "connection", start: { x: col, y: row }, end: { x: pCol, y: row + 1 }, color: parents[p].substr(0, 6) });
        }

        shapes.push({ type: "circle", center: { x: col, y: row }, color: hash.substr(0, 6) });

        maxWidth = Math.max(maxWidth, incoming.length);
    }

    var canvas = $("#graph-canvas")[0];
    var context = canvas.getContext("2d");

    var colWidth = 13;
    var rowHeight = 24;
    var lineWidth = 2;
    var dotRadius = 4;
    var margin = dotRadius * 2;

    canvas.width = margin * 2 + maxWidth * colWidth;
    canvas.height = margin * 2 + data.length * rowHeight;

    for (var i = 0; i < shapes.length; i++) {
        if (shapes[i].type == "connection") {
            var start = shapes[i].start;
            var end = shapes[i].end;
            var color = shapes[i].color;
            context.beginPath();
            context.moveTo(margin + start.x * colWidth, margin + start.y * rowHeight);
            context.bezierCurveTo(margin + start.x * colWidth, margin + end.y * rowHeight - rowHeight / 2, margin + end.x * colWidth, margin + start.y * rowHeight + rowHeight / 2, margin + end.x * colWidth, margin + end.y * rowHeight);
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
            context.strokeStyle = "#000";
            context.lineWidth = 1;
            context.stroke();
        }
    }
});