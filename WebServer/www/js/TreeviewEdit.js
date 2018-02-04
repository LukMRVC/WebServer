
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

function AddNodeToTreeview(name) {
    console.log("Before push: ", myData);
    myData.push({
        text: name
    });
    console.log("After push: ", myData);
    $("#tree").treeview('remove');
    $("#tree").treeview({
        levels: 2,
        data: myData,
        expandIcon: 'fa fa-plus',
        collapseIcon: 'fa fa-minus',
        emptyIcon: 'fa',
        showIcon: true

    });
}
