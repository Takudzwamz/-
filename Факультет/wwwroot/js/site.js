
(function (versh, $, undefined) {

    versh.InitAutocomplete = function (AutoSearch, AutoSearchValue, Url, entityName) {
        $(AutoSearch).autocomplete({
            //A request object, with a single term property, which refers to the value currently in the text input.
            source: function (request, response) {
                $.ajax({
                    url: Url,
                    dataType: "json",
                    type: 'post',
                    data: {
                        pattern: request.term,
                        entityName: entityName
                    },
                    success: function (data) {
                        response(data.data);
                    }
                });
            },
            delay: 500, // задержка после нажатия клавиши и до посылки запроса на сервер за списком
            minLength: 3,

            select: function (event, ui) {
                // select: не работает
            },
            open: function () { $(this).removeClass("ui-corner-all").addClass("ui-corner-top"); },
            close: function () { $(this).removeClass("ui-corner-top").addClass("ui-corner-all"); }
        });

        // При выборе из списка отобразить выбранное значение и сохранить код клиента в скрытом поле
        // (select: не работает - только эта привязка)
        $(AutoSearch).on("autocompleteselect", function (event, ui) {
            this.value = ui.item.label;// отобразить выбранное имя
            $(AutoSearchValue).val(ui.item.value);// для хранения кода 
            $(AutoSearchValue).prop('value', ui.item.value);
            //$(this).trigger('change');
            return false;
        });
    };



// Координаты элемента на странице
// http://javascript.ru/ui/offset
versh.getOffsetRect = function (elem) {
    // (1) Получить ограничивающий прямоугольник для элемента.
    var box = elem.getBoundingClientRect();

    // (2) Задать две переменных для удобства
    var body = document.body;
    var docElem = document.documentElement;

    // (3) Вычислить прокрутку документа. Все браузеры, кроме IE, поддерживают pageXOffset/pageYOffset, а в IE, 
    //при наличии DOCTYPE прокрутка вычисляется либо на documentElement(<html>), иначе на body - что есть то и берем
    var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop;
    var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft;

    // (4) Документ(html или body) бывает сдвинут относительно окна (IE). Получаем этот сдвиг.
    var clientTop = docElem.clientTop || body.clientTop || 0;
    var clientLeft = docElem.clientLeft || body.clientLeft || 0;

    // (5) Прибавляем к координатам относительно окна прокрутку и вычитаем сдвиг html/body, чтобы получить координаты относительно документа
    var top = box.top + scrollTop - clientTop;
    var left = box.left + scrollLeft - clientLeft;

    return { top: Math.round(top), left: Math.round(left) };
};

//========================================== tooltip
versh.simple_tooltip = function (target_items, name) {
    $(target_items).each(function (i) {

        if (document.documentElement.clientWidth <= 600) {
            // remove tooltips
            $(this).removeAttr("title");
            $("." + name).unbind().remove();
        } else {
            $("body").append("<div class='" + name + "' id='" + name + i + "'><p>" + $(this).attr('title') + "</p></div>");
            var my_tooltip = $("#" + name + i);
            var tid = [];
            $(this).removeAttr("title").mouseover(function () {
                my_tooltip.css({ opacity: 0.9, display: "none" }).fadeIn(200);
                tid[i] = setTimeout(function () { my_tooltip.fadeOut(200) }, 3000);
                // console.log(tid.toString())
            }).mousemove(function (kmouse) {
                var ww = $(window).width() / 2;
                var shift;
                if (versh.getOffsetRect(this).left < ww) {
                    shift = 15;
                } else {
                    shift = -15 - $(my_tooltip).width();
                };

                my_tooltip.css({ left: kmouse.pageX + shift, top: kmouse.pageY + 15 });
            }).mouseout(function () {
                // console.log("i=" + i)
                clearTimeout(tid[i]);
                my_tooltip.fadeOut(200);
            });
        } //else

    });
};


// Преобразование даты в другую временную зону
// d - дата-время JavaScript
versh.otherTimeZone = function (d, offset) {
    if (d === null) { return null; }
    if (typeof offset === 'undefined') { offset = 4; } // Московское время
    // convert to msec
    // add local time zone offset
    // get UTC time in msec
    var utc = d.getTime() + (d.getTimezoneOffset() * 60000);
    // create new Date object for different zone
    // using supplied offset
    var nd = new Date(utc + 3600000 * offset);
    return nd;
};

versh.JsonDateToJavaScriptDate = function (data) {
    if (data === null) { return null }
    var numberDate = data.replace(/\/Date\((-?\d+)\)\//, '$1');//выделить число в /Date(1100462400000)/
    if (numberDate === '-62135596800000') { return null }
    var d = new Date(parseInt(numberDate));
    return d;
};

// Преобразование из формата даты JavaScript в нужный текстовый формат 
versh.FormatDate = function (data, dFormat) {
    if (data === null) { return ''; }
    //https://github.com/phstc/jquery-dateFormat  библиотека форматирования jquery.dateFormat-1.0.js
    return $.format.date(data, dFormat)//"dd.MM.yyyy"
};


/////////// СЕРВЕРНАЯ ВАЛИДАЦИЯ ФОРМЫ РЕДАКТИРОВАНИЯ И ДОБАВЛЕНИЯ ЗАДАНИЯ
versh.checkVersh = function (field, rules, i, options, data) {
    if (data !== undefined) {// настроечный вызов
        checkVersh.data = data; // сохраняем в статической checkVersh.data, так как в вызовах валидации data = undefined
    } else {// вызов при валидации
        var formData = $(checkVersh.data.form).serializeFormJSON();//  текущие данные формы в объект formData
        var mergedData = versh.merge_options(checkVersh.data.record, formData);// переопределить поля записи данными формы

        //---------------- 
        var params = {};
        params.record = JSON.stringify(mergedData)// mergedData в JSON
        params.field = $(field).prop('name');
        params.formType = checkVersh.data.formType;
        var res;
        $.ajaxSetup({ async: false, cache: false }); // выполнить синхронно
        $.getJSON('/Tasks/Validation', params, function (data) {
            if (data.Result !== 'OK') {
                alert(data.Message);
                return;
            }
            res = data.Message;

        }).error(function (error) {
            console.log(error);
            var wnd = window.open("about:blank", "", "_blank");
            wnd.document.write(error.responseText);
        });
        $.ajaxSetup({ async: true, cache: true });
        return res;
    }
};
/////////////////////////////////////////////
versh.serializeFormJSON = function (form) {

    var o = {};
    var a = form.serializeArray();
    $.each(a, function () {
        if (o[form.name]) {
            if (!o[form.name].push) {
                o[form.name] = [o[form.name]];
            }
            o[form.name].push(form.value || '');
        } else {
            o[form.name] = form.value || '';
        }
    });
    return o;
};
/////////////////////////////////////////////
versh.merge_options = function (obj1, obj2) {

    //* Overwrites obj1's values with obj2's and adds obj2's if non existent in obj1
    //* @returns obj3 a new object based on obj1 and obj2
    var obj3 = {};
    for (var attrname1 in obj1) { obj3[attrname1] = obj1[attrname1]; }
    for (var attrname2 in obj2) { obj3[attrname2] = obj2[attrname2]; }
    return obj3;
};


    // Здесь задаются параметры функции  versh, $, undefined
    // 1 параметр
    // window.versh = window.versh || {} -> если versh не существует в глобальном пространстве (window), 
    // то первому параметру присваивается пустой объект {}, а если существует, то он используется внутри функции, которая может быть расширением данной функции.
    // Используя такой подход, мы можем построить библиотеку JavaScript, расширяя подобными функциями и в нескольких файлах.
    //  (function (versh, $, undefined) {
    //      // добавляем функциональность в versh
    //  }(window.versh = window.versh || {}, jQuery));

    // 2 параметр
    // передается jQuery, что позволяет использовать синоним $ внутри функции, не заботясь о возможном конфликте с $, объявленном в других библиотеках

    // 3 параметр - не задан
    // так как параметр не задан, то он принимает значение undefined из глобального пространства, 
    // что предотвращает использование переопределенного в других библиотеках undefined
}(window.versh = window.versh || {}, jQuery));

//================================================================
// jQuery plugin
//The jQuery.fn.extend() method extends the jQuery prototype ($.fn) object to provide new methods that can be chained to the jQuery() function.
// Выделение части текста в input (для автозавершение, которое не использую)
$.fn.selectRange = function (start, end) {
    // нужно вернуть всю входную коллекцию, чтобы можно было продолжить цепь функций jQuery
    return this.each(function () {
        if (this.setSelectionRange) {
            this.focus();
            this.setSelectionRange(start, end);
        } else if (this.createTextRange) {
            var range = this.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
        }
    });
};
//================================================================