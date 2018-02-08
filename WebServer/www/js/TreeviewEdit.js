
function initializeTree(object) {
    let i = 0;
    console.log()
    for (i = 0; i < object.Categories.length; ++i) {

        if (object.Categories[i].ParentId == null) {
            AddNodeToTreeview(object.Categories[i].Name)
        }
    }
    $("#tree").treeview({
        levels: 2,
        data: myData,
        expandIcon: 'fa fa-angle-right',
        collapseIcon: 'fa fa-angle-down',
        emptyIcon: 'fa',
        showIcon: true
    });
}


function AddNodeToTreeview(name) {
    myData.push({
        text: "<span class='category'>" + name + "</span>"
    });
}

function AddFoodToTreeview(values) {
    let obj = {
        text: "<span class='food'>"
    };
    for (let i = 0; i < values.length; ++i) {
        obj.text += values[i]; 
    }
    obj.text += "</span>";

    myData.push(obj);
}

function rebuildTreeview() {
    $("#tree").treeview('remove');
    $("#tree").treeview({
        levels: 2,
        data: myData,
        expandIcon: 'fa fa-angle-right',
        collapseIcon: 'fa fa-angle-down',
        emptyIcon: 'fa',
        showIcon: true

    });
}
