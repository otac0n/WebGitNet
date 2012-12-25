$(function () {
    var langOverrides = {
        'cshtml': 'html'
    };

    $('pre[data-extension]').each(function () {
        var lang = $(this).data('extension');
        lang = langOverrides[lang] || lang;
        $(this).addClass('lang-' + lang);
    });

    prettyPrint();
});
