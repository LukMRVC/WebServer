
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

$("#save-food").click((e) =>{
    e.preventDefault();
    let modal = $("#addFoodModal");
    let values = [];
    $("#required-info").find('input').each( (index, ele) => {
        if(!$(ele).val()){
            $(ele).addClass('is-invalid');
        }else{
            if($(ele).hasClass('is-invalid')){
                $(ele).removeClass('is-invalid');
            }
            $(ele).addClass('is-valid');
        }
    });

})