function insertTextAtBegin(text,id) {
    insertAtCaret(text, "", id, 1);
}

function insertTextAtEnd(text, id) {
    insertAtCaret(text, "", id, 2);
}

function replaceText(text, id) {
    insertAtCaret(text, "", id, 3);
}

function insertTextAroundSelection(text,text2, id) {
    insertAtCaret(text, text2, id, 4);
}



function insertAtCaret(text,text2, id, option) {
    var txtarea = document.getElementById(id);
    if (!txtarea) {
        return;
    }

    var scrollPos = txtarea.scrollTop;
    var strPos = 0;
    var strEnd = 0;
    var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ?
        "ff" : (document.selection ? "ie" : false));
    if (br == "ie") {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        strPos = range.text.length;
    } else if (br == "ff") {
        strPos = txtarea.selectionStart;
        strEnd = txtarea.selectionEnd;
    }
    var front = "";
    var back = "";

    if (option == 1) {
        front = (txtarea.value).substring(0, strPos);
        back = (txtarea.value).substring(strPos, txtarea.value.length);
        txtarea.value = front + text + back;
        strPos = strPos + text.length;
        strEnd = strEnd + text.length;
    }
    if (option == 2) {
        front = (txtarea.value).substring(0, strEnd);
        back = (txtarea.value).substring(strEnd, txtarea.value.length);
        txtarea.value = front + text + back;
    }
    if (option == 3) {
        front = (txtarea.value).substring(0, strPos);
        back = (txtarea.value).substring(strEnd, txtarea.value.length);
        txtarea.value = front + text + back;
        strPos = strPos + text.length;
        strEnd = strPos;
    }
    if (option == 4) {
        front = (txtarea.value).substring(0, strPos);
        var middle = (txtarea.value).substring(strPos, strEnd);
        back = (txtarea.value).substring(strEnd, txtarea.value.length);
        txtarea.value = front + text + middle + text2 + back;
        strPos = strPos + text.length;
        strEnd = strEnd + text.length;
    }
    
    if (br == "ie") {
        txtarea.focus();
        var ieRange = document.selection.createRange();
        ieRange.moveStart('character', -txtarea.value.length);
        ieRange.moveStart('character', strPos);
        ieRange.moveEnd('character', 0);
        ieRange.select();
    } else if (br == "ff") {
        txtarea.selectionStart = strPos;
        txtarea.selectionEnd = strEnd;
        txtarea.setSelectionRange(strPos, strEnd);
        txtarea.focus();
    }

    txtarea.scrollTop = scrollPos;
}