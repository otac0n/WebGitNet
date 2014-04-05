/// <reference path="jquery-1.4.4-vsdoc.js" />

function readValues(column, normalize) {
    normalize = normalize || function (a) { return a; };

    var headers = $("#stats-table thead th").get();
    var getIndex = function (headerText) {
        return headers.indexOf($.map(headers, function (h) { if ($(h).text() === headerText) { return h; } })[0]);
    };

    var rows = $("#stats-table tbody tr");
    var getValues = function (headerText, normalize) {
        var colIndex = getIndex(headerText);
        return $.map(rows, function (item) { return normalize($($(item).find("td")[colIndex]).text()); });
    };

    return getValues(column, normalize);
}

function buildGraph(div) {
    var labels = readValues("Author");
    var commits = readValues("Commits", function (a) { return +a; });
    var insertions = readValues("Insertions", function (a) { return +a; });
    var deletions = readValues("Deletions", function (a) { return +a; });
    var impact = readValues("Impact", function (a) { return +a; });

    var hIn = function () {
        this.sector.stop();
        this.sector.scale(1.1, 1.1, this.cx, this.cy);
        if (this.label) {
            this.label[0].stop();
            this.label[0].scale(1.5);
            this.label[1].attr({ "font-weight": 800 });
        }
    };

    var hOut = function () {
        this.sector.animate({ scale: [1, 1, this.cx, this.cy] }, 500, "bounce");
        if (this.label) {
            this.label[0].animate({ scale: 1 }, 250, "bounce");
            this.label[1].attr({ "font-weight": 400 });
        }
    };

    var r = Raphael(div, 940, 220);
    r.g.text(250, 20, "Commits").attr({ "font-size": 20 });
    r.g.text(720, 20, "Impact").attr({ "font-size": 20 });
    r.g.piechart(110, 110, 100, commits, { legend: labels, legendpos: "east" }).hover(hIn, hOut);
    r.g.piechart(580, 110, 100, impact, { legend: labels, legendpos: "east" }).hover(hIn, hOut);
}

$(function () {
    impact("weekly-graph", data, { heightfunc: Math.sqrt });

    buildGraph("users-dashboard");
});