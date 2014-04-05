/// <reference path="~/Scripts/jquery-2.1.0.intellisense.js" />
/// <reference path="~/Scripts/jquery-ui-1.8.18.js" />
/// <reference path="~/Scripts/jquery.cookie.js" />
/// <reference path="~/Scripts/json2.js" />

var Graph = {};

Graph.defaults = {
    colWidth: 13,
    rowHeight: 24,
    lineWidth: 2,
    curveLine: true,
    dotRadius: 3,
    dotBorder: 0,
    margin: 10,
    rightAlign: true,
    flushLeft: false,
    usePalette: true,
    palette: [
        "0061B0",
        "911822",
        "CCAD49",
        "439959",
        "A01E86",
        "875B0E",
        "EA4517",
        "2B14AD",
        "3E6000",
        "68727F",
        "000000"
    ]
};

Graph.init = function () {
    Graph.options = JSON.parse($.cookie("Graph.options")) || {};

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

    Graph.data = [];
    $(".graph-node").each(function () {
        Graph.data.push({
            node: node($(this).data('node')),
            parents: split($(this).data('parent-nodes')),
            incoming: split($(this).data('incoming-nodes')),
            div: this
        });
    });
};

Graph.render = function () {
    var data = this.data;
    var options = $.extend({}, this.defaults, this.options);

    options.margin = Math.max((options.dotRadius + options.dotBorder) * 2, options.margin);

    var find = function (list, predicate) {
        for (var i = 0; i < list.length; i++) {
            if (predicate(list[i])) {
                return i;
            }
        }

        return -1;
    };

    var color = function (node) {
        if (options.usePalette) {
            return options.palette[node.color % options.palette.length];
        } else {
            return node.hash.substr(0, 6);
        }
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

    canvas.width = options.margin * 2 + maxWidth * options.colWidth;
    canvas.height = options.margin * 2 + (data.length - 1) * options.rowHeight;

    // Position the text.
    for (var y = 0; y < data.length; y++) {
        $div = $(data[y].div);
        var h = $div.height();
        $div.css({
            position: "absolute",
            top: (options.margin + y * options.rowHeight - h / 2) + "px",
            left: (options.margin + (options.rightAlign || options.flushLeft ? maxWidth : data[y].incoming.length) * options.colWidth) + "px"
        });
    }

    var map = function (location) {
        return {
            x: options.margin + (options.rightAlign ? maxWidth - location.x - 1 : location.x) * options.colWidth,
            y: options.margin + location.y * options.rowHeight
        };
    };

    // Draw the lines and nodes.
    for (var i = 0; i < shapes.length; i++) {
        if (shapes[i].type == "connection") {
            var start = map(shapes[i].start);
            var end = map(shapes[i].end);
            var color = shapes[i].color;
            context.beginPath();
            context.moveTo(start.x, start.y);
            if (options.curveLine) {
                context.bezierCurveTo(start.x, end.y - options.rowHeight / 2, end.x, start.y + options.rowHeight / 2, end.x, end.y);
            } else {
                context.lineTo(end.x, end.y);
            }
            context.strokeStyle = "#" + color;
            context.lineWidth = options.lineWidth;
            context.stroke();
        } else if (shapes[i].type == "circle") {
            var center = map(shapes[i].center);
            var color = shapes[i].color;
            context.beginPath();
            context.arc(center.x, center.y, options.dotRadius, 0, 2 * Math.PI, false);
            context.fillStyle = "#" + color;
            context.fill();
            if (options.dotBorder) {
                context.strokeStyle = "#000";
                context.lineWidth = options.dotBorder;
                context.stroke();
            }
        }
    }
};

$(function () {
    Graph.init();
    Graph.render();
});

$(function () {
    var suppressSave = false;
    var save = function (value) {
        if (!suppressSave) {
            $.extend(Graph.options, value);
            $.cookie("Graph.options", JSON.stringify(Graph.options), { path: '/', expires: 365 });
            Graph.render();
        }
    };

    var resetUI = function () {
        var options = $.extend({}, Graph.defaults, Graph.options);
        suppressSave = true;
        $("input[name='align']").each(function () {
            $(this).prop("checked", $(this).val() == (options.rightAlign ? "right" : "left"));
        });
        $("input[name='outline']").prop("checked", !options.flushLeft);
        $("input[name='lineWidth']").val(options.lineWidth);
        $("input[name='curveLine']").prop("checked", options.curveLine);
        $("input[name='dotRadius']").val(options.dotRadius);
        $("input[name='dotBorder']").val(options.dotBorder);
        $("input[name='colWidth']").val(options.colWidth);
        $("input[name='rowHeight']").val(options.rowHeight);
        suppressSave = false;
    }

    $("#reset-settings").click(function () {
        Graph.options = {};
        $.cookie("Graph.options", null, { path: '/', expires: 365 });
        Graph.render();
        resetUI();
        $("#graph-settings").modal('toggle');
    });

    $("input[name='align']").change(function () {
        save({ rightAlign: $("input[name='align']:checked").val() == "right" });
    });

    $("input[name='outline']").change(function () {
        save({ flushLeft: !$(this).prop("checked") });
    });

    $("input[name='lineWidth']").change(function () {
        save({ lineWidth: +$(this).val() });
    });

    $("input[name='curveLine']").change(function () {
        save({ curveLine: $(this).prop("checked") });
    });

    $("input[name='dotRadius']").change(function () {
        save({ dotRadius: +$(this).val() });
    });

    $("input[name='dotBorder']").change(function () {
        save({ dotBorder: +$(this).val() });
    });

    $("input[name='colWidth']").change(function () {
        save({ colWidth: +$(this).val() });
    });

    $("input[name='rowHeight']").change(function () {
        save({ rowHeight: +$(this).val() });
    });

    resetUI();
});