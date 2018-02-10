
function initializeTree(object) {
    let i = 0;
    console.log()
    for (i = 0; i < object.Categories.length; ++i) {

        if (object.Categories[i].ParentId == null) {
            AddNodeToTreeview(object.Categories[i])
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


function AddNodeToTreeview(category, parentName) {
    //Fix this shit, after adding subcategory, indices in myData dont match
    if (category.ParentId == null) {
        myData.push({
            text: "<span class='category'>" + category.Name + "</span>"
        });
    } else {
        let objToAppend = FindParent(myData, parentName);
        let parent = myData[parseInt(category.ParentId)];
        if (!objToAppend.hasOwnProperty('nodes'))
            objToAppend.nodes = [];
        objToAppend.nodes.push({
            text: "<span class='category'>" + category.Name + "</span>"
        });
       /* myData[parseInt(category.ParentId)].nodes = [];
        myData[parseInt(category.ParentId)].nodes.push({
            text: "<span class='category'>" + category.Name + "</span>"
        });*/
    }
    RebuildTreeview();
    populateCategories();
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

function RebuildTreeview() {
    $("#tree").treeview('remove');
    $("#tree").treeview({
        levels: 5,
        data: myData,
        expandIcon: 'fa fa-angle-right',
        collapseIcon: 'fa fa-angle-down',
        emptyIcon: 'fa',
        showIcon: true
    });
    $('#tree').treeview('expandAll', { silent: true });
}

function FindParent(nodes, name) {
    //object to append then
    let ret = null;

    for (let i = 0; i < nodes.length; ++i) {
        if (nodes[i].text.includes(name)) {
            ret = nodes[i];
            break
        }
        if (nodes[i].hasOwnProperty('nodes')) {
            ret = FindParent(nodes[i].nodes, name);
            if (ret === null) {
                continue;
            } else {
                break;
            }
        }
    }
    return ret;
}