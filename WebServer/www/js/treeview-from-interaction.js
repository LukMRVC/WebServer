$("#add-food").click((e) => {
    $("#addFoodModal").modal();
});
$("#add-category").click(() => {
    $("#categoryModal").modal();
});


$("#categoryModal").on('show.bs.modal', function (ev) {
    let options = [];
    $("#tree").find("li").each((index, ele) => {
        let option = {};
        if ($(ele).has(".category")) {
            option.value = $(ele).attr('data-nodeid');
            option.name = $(ele).children(".category").html();
            options.push(option);
        }

    });
    console.log(options);
    let modal = $(this);
    let $select = modal.find("#categorySelect");
    console.log($select);
    for (let i = 0; i < options.length; ++i) {
        $select.append($('<option>', {
            value: options[i].value,
            text: options[i].name
        }));
    }
});