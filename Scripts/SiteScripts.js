$(document).ready(function () {
    $('.phone').mask('(999) 999-9999');
    $('.phoneExt').mask('(999) 999-9999 poste 99999');

    $(".datepicker").datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        dayNamesMin: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"],
        monthNamesShort: ["Janv.", "Févr.", "Mars", "Avril", "Mai", "Juin", "Juil.", "Août", "Sept.", "Oct.", "Nov.", "Déc."]
    });

    $(":input").change(function () {
        try {
            let r = $(this).val().replace(/[^\x00-\xFF]/g, "");
            $(this).val(r);
        }
        catch (e) { }
    });

    $("textarea").change(function () {
        try {
            let r = $(this).val().replace(/[^\x00-\xFF]/g, "");
            $(this).val(r);
        }
        catch (e) { }
    });

    SummaryHandling();
    RestoreDetailsState();

    $(".submitCmd").click(function () {
        $("form").submit();
    });
});

function SummaryHandling() {
    $('summary').attr('title', 'Utilisez ctrl-clic pour développer/réduire');
    $('summary').off();

    $('summary').on('click', function (e) {
        if (e.ctrlKey) {
            if ($(this).parent().attr('open') != undefined) {
                $('details').removeAttr('open');
                e.preventDefault();
            }
            else {
                $('details').prop('open', true);
                e.preventDefault();
            }
        }
    });
}

function RestoreDetailsState() {
    $("details").off();

    $("details").on('toggle', function () {
        let details_dom = $(this)[0];

        if (details_dom != undefined && details_dom.id != undefined && details_dom.id.indexOf("details") > -1) {
            localStorage.setItem(details_dom.id, details_dom.open);
        }
    });

    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);

        if (key.indexOf("details") > -1) {
            let details_dom = $("#" + key)[0];

            if (details_dom != undefined) {
                details_dom.open = localStorage.getItem(key) == "true";
            }
        }
    }
}

function ajaxActionCall(actionLink) {
    $.ajax({
        url: actionLink,
        method: 'GET'
    });
}

function setStudentSearchString(value) {
    $.ajax({
        url: "/Students/SetSearchString",
        method: "GET",
        data: { searchString: value }
    });
}

function setSelectedStudentYear(value) {
    $.ajax({
        url: "/Students/SetSelectedYear",
        method: "GET",
        data: { year: value }
    });
}

function setCourseSearchString(value) {
    $.ajax({
        url: "/Courses/SetSearchString",
        method: "GET",
        data: { searchString: value }
    });
}

function setTeacherSearchString(value) {
    $.ajax({
        url: "/Teachers/SetSearchString",
        method: "GET",
        data: { searchString: value }
    });
}

function normalizeString(value) {
    if (value == undefined || value == null) {
        return "";
    }

    return value.toString().toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

function highlight(text, elem) {
    text = normalizeString(text).trim();

    if (text.length > 0) {
        let innerHTML = elem.innerHTML;
        let plain = normalizeString(innerHTML);
        let startIndex = 0;

        while (startIndex < innerHTML.length) {
            plain = normalizeString(innerHTML);
            let index = plain.indexOf(text, startIndex);

            if (index >= startIndex) {
                let highLightedText = "<span class='highlight'>" + innerHTML.substring(index, index + text.length) + "</span>";
                innerHTML = innerHTML.substring(0, index) + highLightedText + innerHTML.substring(index + text.length);
                startIndex = index + highLightedText.length;
            }
            else {
                startIndex = innerHTML.length + 1;
            }
        }

        elem.innerHTML = innerHTML;
    }
}

function highlightAll(searchString, targetClass) {
    searchString = normalizeString(searchString).trim();

    if (searchString.length > 0) {
        $("." + targetClass).each(function () {
            highlight(searchString, this);
        });
    }
}

function InstallAutoComplete(targetId, words) {
    function split(val) {
        return val.split(/ \s*/);
    }

    function RemoveExtra(str, extra) {
        var extraLength = extra.length;
        var lastExtraIndex = str.lastIndexOf(extra);

        if ((lastExtraIndex + extraLength) == str.length) {
            str = str.substring(0, str.length - extraLength);
        }

        return str;
    }

    function extractLast(term) {
        return split(term).pop();
    }

    $("#" + targetId)
        .bind("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB && $(this).data("ui-autocomplete").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            minLength: 1,
            source: function (request, response) {
                response($.ui.autocomplete.filter(words, extractLast(request.term)));
            },
            focus: function () {
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                terms.pop();
                terms.push(ui.item.value);
                terms.push("");
                this.value = RemoveExtra(terms, ",").join(" ");
                return false;
            }
        });
}