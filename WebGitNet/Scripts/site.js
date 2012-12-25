$(function () {
    $('pre[data-extension]').each(function () {
        $(this).addClass('lang-' + $(this).data('extension'));
    });

    prettyPrint();
});
