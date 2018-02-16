$("#add-food").click((e) => {
    $("#addFoodModal").modal();
});
$("#add-category").click(() => {
    $("#categoryModal").modal();
});

$("#remove").click(() => {
    let $selected = $("#tree").find('.node-selected');
    if ($selected.length == 0) {
        alert("No node was selected");
        return;
    } else {
        if (confirm("Jste si jistý?")) {
            Remove( $($selected[0]) );
        }
    }
});

function populateCategories() {
    let modal = $("#categoryModal");
    let $select = modal.find("#categorySelect");
    let $addFoodSelect = $("#addFoodModal").find("#catSelect");
    $select.find('option').remove();
    $addFoodSelect.find('option').remove();
    let options = [];
    let nullOption = {
        value: "null",
        name: "Vybrat..."
    }
    options.push(nullOption);
    //Opraveno
    $("#tree .list-group").find("[data-nodeid]").each((index, ele) => {
        let option = {};
        //if ($(ele).has(".category")) {
        if ($(ele).find(".category")) {
            option.value = $(ele).attr('data-nodeid');
            option.name = $(ele).find(".category").html();
            if (!option.name)
                return;
            options.push(option);
        }

    });
    
    for (let i = 0; i < options.length; ++i) {
        $select.append($('<option>', {
            value: options[i].value,
            text: options[i].name,
        }));
        $addFoodSelect.append($('<option>', {
            value: options[i].value,
            text: options[i].name,
        }));
    }
}

function update(event, foodId) {
    //Dát do modalu hidden ID na úpravu, samozřejmě najít food z referencí, aby se mohli do modalu hodit data
    event.preventDefault();
    for (let i = 0; i < FoodReferences.length; ++i) {
        if (FoodReferences[i].Id === foodId) {
            let modal = $("#addFoodModal");
            modal.find("#food-id").val(FoodReferences[i].Id);
                //Ještě najít kategorii

            for (let k = 0; k < CategoryReferences.length; ++k) {
                if (CategoryReferences[k].Id === FoodReferences[i].CategoryId) {
                    modal.find("#catSelect option").each((ind, e) => {
                        if ($(e).text() == CategoryReferences[k].Name) {
                            $(e).attr('selected', 'selected');
                            return;
                        }
                    });
                    break;
                }
            }
            modal.find(".form-control").each((index, element) => {

                for (let key in FoodReferences[i]) {
                    if (!FoodReferences[i].hasOwnProperty(key)) continue;

                    if (key.toLowerCase() == $(element).attr('id').substring(5).toLowerCase()) {
                        $(element).val(FoodReferences[i][key]);
                    }

                }
                console.log();
            });

            modal.data("update", FoodReferences[i].Name);

            modal.modal("show");
            break;
        }
    }
}

function Remove(object) {
    console.log(object);
    if (object.children().hasClass('category')) {
        let id = object.children('.category').attr('data-id');
        console.log("id: ", id);
        $.ajax({
            url: '/api/category/delete/' + id,
            type: 'DELETE',
            success: RemoveFromTreeview(object)
        });
    } else {
        let id = object.children('.food').attr('data-id');
        $.ajax({
            url: '/api/food/delete/' + id,
            type: 'DELETE',
            success: RemoveFromTreeview(object)
        });
        console.log("food");
    }
}

function FindReference() {
    return 0;
}