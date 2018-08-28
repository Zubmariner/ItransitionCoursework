$(document).ready(function checkAll() {
    $("#CheckParent").click(function () {
        $(".CheckChild").prop("checked", this.checked);
    });

    $(".CheckChild").click(function () {
        if ($(".CheckChild:checked").length === $(".CheckChild").length) {
            $("#CheckParent").prop("checked", true);
        } else {
            $("#CheckParent").prop("checked", false);
        }
    });
});

$(document).ready(function () {

    $("#DeleteButton").prop("disabled", true);
    $("#BlockButton").prop("disabled", true);
    $("#UnblockButton").prop("disabled", true);
    $(".CheckChild").add("#CheckParent").click(function () {

        var isNoneChecked = $(".CheckChild:checked").length === 0;
        $("#DeleteButton").prop("disabled", isNoneChecked);
        $("#BlockButton").prop("disabled", isNoneChecked);
        $("#UnblockButton").prop("disabled", isNoneChecked);
    });
});

$(document).ready(function () {
    $("#changeTheme").click(function() {

        if ($(this).hasClass("fa-moon")) {
            $(this).removeClass("fa-moon").addClass("fa-sun");
            $("#themeStyle").prop("href", "/lib/bootstrap/dist/css/bootstrap.css");
        } else {
            $(this).removeClass("fa-sun").addClass("fa-moon");
            $("#themeStyle").prop("href", "/lib/bootstrap/dist/css/bootstrap-darkly.css");
        }

        var theme;
        if ($(this).hasClass("fa-moon"))
            theme = "light";
        else theme = "dark";

        $.post("/Home/SetTheme",
            {
                theme: theme
            });
    });
});

$(document).ready(function () {
    $(".likeComment").click(function () {
        $.post("/Instruction/LikeComment",
            {
                userId: $(this).data("userId"),
                commentId: $(this).data("commentId")
            });
    });
});