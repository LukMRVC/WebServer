
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
    let allergens = [];
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

    let invalid = false;
    $("#nutrition-info").find('input').each((index, ele) => {
        if ($(ele).val() && !$(ele).is(":checkbox")) {
            if (isNaN($(ele).val()) ) {
                $(ele).addClass('is-invalid');
                invalid = true;
                return;
            }
            propName = $(ele).attr('id').charAt(5).toUpperCase() + $(ele).attr('id').substring(6);
            food[propName] = $(ele).val();
            values.push($(ele).val());
            if ($(ele).hasClass('is-invalid')) {
                $(ele).removeClass('is-invalid');
            }
            $(ele).addClass('is-valid');

        } else if ($(ele).is(":checked")) {
            allergens.push($(ele).val().substring(9));
        }
    });
    if (invalid)
        return;
    food['Allergens'] = allergens;
    console.log(food);

    $.ajax({
        url: "/api/food/add",
        type: "POST",
        data: food,

    })
});


function validate() {
    let node = $("#tree").find(".node-selected");
    if (node.length == 0);
    return false;
    console.log(node);
}