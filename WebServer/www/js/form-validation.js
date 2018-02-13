
$("#save-category").click((e) => {
    e.preventDefault();
    let modal = $("#categoryModal");
    
    let parent = modal.find("#categorySelect").val();
    let value = modal.find('.modal-body input').val();
    let parentName = modal.find(":selected").text();
    if (value) {
        if (modal.find('.modal-body input').hasClass('is-invalid')) {
            modal.find('.modal-body input').removeClass('is-invalid');
        }
    } else {
        modal.find('.modal-body input').addClass('is-invalid');
        return;
    }
    modal.find('.modal-body input').addClass('is-valid');
    if (parent === "null")
        parent = null;
    let body = {
        Name: value,
        ParentId : parent,
        ParentName: parentName
    };
    if (modal.find("#category-id").val() != "null") {
        body.CategoryId = modal.find("#category-id").val();
    }
    if (value) {
        $.ajax({
            url: "/api/category/add",
            data: JSON.stringify(body),
            error: (e) => { console.log(e); },
            type: (body.hasOwnProperty('CategoryId')) ? "PUT" : "POST",
            success: (result) => {
                CategoryReferences.push(result);
                AddNodeToTreeview(body, parentName)
            }
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
    if ($("#catSelect").val() == "null") {
        $("#catSelect").addClass('is-invalid');
        return;
    } else if ($("#catSelect").hasClass('is-invalid')) {
        $("#catSelect").removeClass('is-invalid');
        $("#catSelect").addClass('is-valid');
    }
    let catName = modal.find(":selected").text();
    //Find category DB id
    for (let i = 0; i < CategoryReferences.length; ++i) {
        if (CategoryReferences[i].Name == catName) {
            food['CategoryId'] = CategoryReferences[i].Id;
            break;
        }
    }
    debugger
    $("#required-info").find('.form-control').each((index, ele) => {
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

    if (modal.find("#food-id").val() != "null") {
        food.foodId = modal.find("#food-id").val();
    }
    debugger
    $.ajax({
        url: "/api/food/add",
        type: (food.hasOwnProperty('foodId')) ? "PUT" : "POST",
        data: JSON.stringify(food),
        success: (result) => {
            console.log("Result: ", result);
            food.Id = result.Id;
            FoodReferences.push(food);
            modal.modal('hide');
            AddFoodToTreeview(food, catName);
        }
    });

});


function validate() {
    let node = $("#tree").find(".node-selected");
    if (node.length == 0);
    return false;
    console.log(node);
}