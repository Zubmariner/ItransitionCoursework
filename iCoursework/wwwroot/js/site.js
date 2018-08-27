$(document).ready(function checkAll() {
    $("#CheckParent").click(function () {
        $(".CheckChild").prop("checked", this.checked);
    });

    $('.CheckChild').click(function () {
        if ($('.CheckChild:checked').length == $('.CheckChild').length) {
            $('#CheckParent').prop('checked', true);
        } else {
            $('#CheckParent').prop('checked', false);
        }
    });
});

$(document).ready(function () {

    $('#DeleteButton').prop('disabled', true);
    $('#BlockButton').prop('disabled', true);
    $('#UnblockButton').prop('disabled', true);
    $('.CheckChild').add('#CheckParent').click(function () {

        var isNoneChecked = $('.CheckChild:checked').length == 0;
        $('#DeleteButton').prop('disabled',
            function (i, val) {
                return isNoneChecked;
            });
        $('#BlockButton').prop('disabled',
            function (i, val) {
                return isNoneChecked;
            });
        $('#UnblockButton').prop('disabled',
            function (i, val) {
                return isNoneChecked;
            });
    });
});