$("#add-food").click((e) => {
    $("#addFoodModal").modal();
});
$("#add-category").click(() => {
    $("#categoryModal").modal();
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
    console.log();

    let element = $(event.target).prev();
    console.log(element.data());


}