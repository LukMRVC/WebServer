
$("#save-category").click((e) => {
    e.preventDefault();
    let modal = $("#categoryModal");
    let value = modal.find('.modal-body input').val();
    let body = {
        name: value
    };
    if (value) {
        $.ajax({
            url: "/api/category/add",
            data: JSON.stringify(body),
            error: (e) => { console.log(e); },
            type: "POST",
            success: AddNodeToTreeview(value)
        });
    }
    modal.modal('hide');
});

$("#save-food").click((e) => {
    e.preventDefault();
    let modal = $("#addFoodModal");
    let values = [];
    let food = {};
    let propName;
    $("#required-info").find('input').each((index, ele) => {
        if (!$(ele).val()) {
            $(ele).addClass('is-invalid');
        }
        else {
            if ($(ele).hasClass('number-only') && (isNaN($(ele).val()))) {
                $(ele).addClass('is-invalid');
                return;
            }

            if ($(ele).hasClass('is-invalid')) {
                $(ele).removeClass('is-invalid');
            }

            $(ele).addClass('is-valid');

            propName = $(ele).attr('id').charAt(5).toUpperCase() + $(ele).attr('id').substring(6);
            food[propName] = $(ele).val();
            values.push($(ele).val());
        }
    });
    if (values.length != 4) {
        return;
    }
    console.log(food);
    $("#nutrition-info").find('input').each((index, ele) => {
        if ($(ele).val() && !isNaN($(ele).val())) {
            propName = $(ele).attr('id').charAt(5).toUpperCase() + $(ele).attr('id').substring(6);
            food[propName] = $(ele).val();
            values.push($(ele).val());
        }
    });
    console.log(values);
});