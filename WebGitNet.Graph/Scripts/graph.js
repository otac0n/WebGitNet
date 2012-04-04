$(function () {
    var $divs = $(".graph-node");

    var data = [];
    $divs.each(function () {
        data.push({ hash: $(this).data('hash'), incoming: $(this).data('incoming-hashes').split(','), div: this });
    });
});