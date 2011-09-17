/// <reference path="jquery-1.4.4-vsdoc.js" />

function buildGraph(div, column) {
    var headers = $("#stats-table thead th").get();
    var getIndex = function (headerText) {
        return headers.indexOf($.map(headers, function (h) { if ($(h).text() === headerText) { return h; } })[0]);
    };

    var rows = $("#stats-table tbody tr");
    var getValues = function (headerText, normalize) {
        normalize = normalize || function (a) { return a; };
        var colIndex = getIndex(headerText);
        return $.map(rows, function (item) { return normalize($($(item).find("td")[colIndex]).text()); });
    };

    var names = getValues("Author");
    var values = getValues(column, function (a) { return +a; });

    var r = Raphael(div);
    r.g.text(250, 20, column).attr({ "font-size": 20 });
    var pie = r.g.piechart(110, 110, 100, values, { legend: names, legendpos: "east" })
    pie.hover(function () {
        this.sector.stop();
        this.sector.scale(1.1, 1.1, this.cx, this.cy);
        if (this.label) {
            this.label[0].stop();
            this.label[0].scale(1.5);
            this.label[1].attr({ "font-weight": 800 });
        }
    }, function () {
        this.sector.animate({ scale: [1, 1, this.cx, this.cy] }, 500, "bounce");
        if (this.label) {
            this.label[0].animate({ scale: 1 }, 500, "bounce");
            this.label[1].attr({ "font-weight": 400 });
        }
    });
}
