// script pour l'interface de gestion de sélection avec deux <select...>
// auteur : Nicolas Chourot

$(document).ready(initUI);

function initUI() {
    sortAllSelect();
    deSelectAll($('body'));

    $('.UnselectedItems').change(function (e) {
        let parent = $(this).parent();

        parent.find('.UnselectedItems option:selected').each(function () {
            parent.find(".SelectedItems option:selected").prop("selected", false);
            parent.find('.AddSelection').show();
            parent.find('.RemoveSelection').hide();
            parent.find('.UnselectAll').show();
        });

        e.preventDefault();
    });

    $('.SelectedItems').change(function (e) {
        let parent = $(this).parent();

        parent.find('option:selected').each(function () {
            parent.find(".UnselectedItems option:selected").prop("selected", false);
            parent.find('.AddSelection').hide();
            parent.find('.RemoveSelection').show();
            parent.find('.UnselectAll').show();
        });

        e.preventDefault();
    });

    $(document).on('submit', 'form', function () {
        $('.SelectedItems option').prop('selected', true);
    });

    $(".AddSelection").on('click', function () {
        let parent = $(this).parent().parent();

        parent.find('.UnselectedItems').first().find('option:selected').each(function () {
            $(this).remove();
            parent.find('.SelectedItems').first().append($(this));
            sortSelect(parent.find(".SelectedItems").first());
            parent.find(".SelectedItems").focus();
        });

        deSelectAll(parent);
    });

    $(".RemoveSelection").on('click', function () {
        let parent = $(this).parent().parent();

        parent.find('.SelectedItems').first().find('option:selected').each(function () {
            $(this).remove();
            parent.find('.UnselectedItems').first().append($(this));
            sortSelect(parent.find(".UnselectedItems").first());
            parent.find(".UnselectedItems").focus();
        });

        deSelectAll(parent);
    });

    $(".UnselectAll").on('click', function () {
        let parent = $(this).parent().parent();
        deSelectAll(parent);
    });
}

function deSelectAll(parent) {
    parent.find('.AddSelection').hide();
    parent.find('.RemoveSelection').hide();
    parent.find('.UnselectAll').hide();
    parent.find('.SelectedItems option').prop('selected', false);
    parent.find('.UnselectedItems option').prop('selected', false);
}

function normalize(str) {
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

function sortSelect(select) {
    select.each(function () {
        let select = $(this);

        select.html(select.find('option').sort(function (option1, option2) {
            return normalize($(option1).text()) < normalize($(option2).text()) ? -1 : 1;
        }));
    });
}

function sortAllSelect() {
    $('select').each(function () {
        let select = $(this);

        select.html(select.find('option').sort(function (option1, option2) {
            return normalize($(option1).text()) < normalize($(option2).text()) ? -1 : 1;
        }));
    });
}