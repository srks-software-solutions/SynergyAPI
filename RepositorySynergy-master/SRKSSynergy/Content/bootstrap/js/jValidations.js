//jvalidation for entering ONLY ALPHABETS & Space and Backspace
$('.j_text').keypress(function (e) {
    var regex = new RegExp("^[a-zA-Z ]$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (!regex.test(str)) {
        e.preventDefault();
        alert("Alphabets Only")
        //$(event.target).next("span").html("Alphabets Only. Maximum:20 ").show().fadeOut(2000);
        return false;
    }
    return true;
});

//jAllow all alphabets and few special characters
$('.j_all').keyup(function (e) {
    //!”$%&’()*\+,\/;\[\\\]\^_`{|}~
    var regex = new RegExp("^[a-zA-Z]$!”$%&’()*\+,\/;\[\\\]\^_`{|}~");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    // alert(str)
    if (!regex.test(str)) {
        e.preventDefault();
        $(event.target).next("span").html("All Characters.").show().fadeOut(3000);
        return false;
    }
    return true;
});

//jAllow only Numbers   
$('.j_int').keypress(function (e) {
    var regex = new RegExp("^[0-9]$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (!regex.test(str)) {
        e.preventDefault();
        $(event.target).next("span").html("Numbers Only.").show().fadeOut(40000);
        return false;
    }
    return true;
});

//jFor restricting any field to its maxlength 
// $(document).ready(function () {
$('.j_length').keyup(function (e) {
    var len = $(this).attr("maxlength");
    if (this.value.length == $(this).attr("maxlength")) {
        e.preventDefault();
        var len = $(this).attr("maxlength");
        alert(" Maximum Length allowed is: " + len)
        return false;
    }
    return true;
});
//  });

//jFor restricting any field to its minlength 
// $(document).ready(function () {
$('.j_minlength').focusout(function (e) {
    var len = $(this).attr("MinLength");
    if (this.value.length < $(this).attr("MinLength")) {
        e.preventDefault();
        var len = $(this).attr("MinLength");
        alert(" Minimum Length allowed is: " + len)
        return false;
    }
    return true;
});
//  });

//jMobile number to start with 7 or 8 or 9
$(".j_mobilenumber").keydown(function (e) {
    if ($(this).value != "") {
        var y = $(this).val();
        if (y.charAt(0) == "9" || y.charAt(0) == "8" || y.charAt(0) == "7")
            return true;
        else {
            $(this).val("");
            $(this).focus();
            alert("Mobile number should start with 7 or 8 or 9")
            return false;
        }
    }
});

//jEmail validation
$('.j_mailid').focusout(function (e) {
    var inputVal = $(this).val();
   // alert(inputVal)
    var emailReg = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!emailReg.test(inputVal)) {
        alert("Please enter something like abc@xyz.com")
        $(this).focus();
    //    // $(event.target).next("span").html("abc@gmail.com").show().fadeOut(2000);
    }
});




