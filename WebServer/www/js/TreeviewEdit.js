
function initializeTree(object) {
    let i = 0;
    console.log()
    for (i = 0; i < object.Categories.length; ++i) {
        if (object.Categories[i].ParentId == null) {
            AddNodeToTreeview(object.Categories[i])
        } else {
            for (let j = 0; j < object.Categories.length; ++j) {
                if (object.Categories[i].ParentId == object.Categories[j].Id) {
                    AddNodeToTreeview(object.Categories[i], object.Categories[j].Name);
                    break;
                }
            }
        }
    }

    for (i = 0; i < object.Food.length; ++i) {
        for (let j = 0; j < object.Categories.length; ++j) {
            if (object.Food[i].CategoryId == object.Categories[j].Id) {
                AddFoodToTreeview(object.Food[i], object.Categories[j].Name);
                break;
            }
        }
    }


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


function AddNodeToTreeview(category, parentName) {
    //Fix this shit, after adding subcategory, indices in myData dont match
    if (category.ParentId == null) {
        myData.push({
            text: "<span class='category' data-id=" + category.Id + ">" + category.Name + "</span>"
        });
    } else {
        let objToAppend = FindParent(myData, parentName);
        let parent = myData[parseInt(category.ParentId)];
        if (!objToAppend.hasOwnProperty('nodes'))
            objToAppend.nodes = [];
        objToAppend.nodes.push({
            text: "<span class='category' data-id=" + category.Id + ">" + category.Name + "</span>"
        });
       /* myData[parseInt(category.ParentId)].nodes = [];
        myData[parseInt(category.ParentId)].nodes.push({
            text: "<span class='category'>" + category.Name + "</span>"
        });*/
    }
    RebuildTreeview();
    populateCategories();
}

function UpdateNodeToTreeview(node, parentName) {
    RemoveNode(myData, node.name);
    AddNodeToTreeview(node, parentName);
}

function AddFoodToTreeview(food, parentName) {
    let obj = "<span class='food' data-id=" + food.Id + ">";

  /*  for (let key in food) {
        if (!food.hasOwnProperty(key)) continue;
        obj += "data-" + key.toLowerCase() + "='" + food[key] + "' ";
    }*/
    obj += "<span class='food-divider name'>" + food.Name + "</span><span class='food-divider'> " + food.Price + "Kč </span><span class='food-divider'>" + food.Gram + "g</span> </span> " +
        " <button type='button' onclick='update(event, "+ food.Id + ")' class='btn btn-primary food-update'>Upravit <i class='fa fa-pencil-square-o'></i></button> ";
    let objToAppend = FindParent(myData, parentName);
    if (!objToAppend.hasOwnProperty('nodes'))
        objToAppend.nodes = [];
    objToAppend.nodes.push({
        text: obj
    });
    RebuildTreeview();
    populateCategories();
}

function UpdateFoodToTreeview(food, parentName, previousName) {
    //Upon changing name, new node is appended to a tree, but it shouldn't, fix that
    RemoveNode(myData, previousName);
    AddFoodToTreeview(food, parentName)
}

function RemoveFromTreeview(node) {
    let obj;
    if (node.children().hasClass('category')) {
        obj = RemoveNode(myData, node.children('.category').text());
    } else {
        obj = RemoveNode(myData, node.children('.food').children('.name').text());
    }
   
    RebuildTreeview();
}


function RebuildTreeview() {
     //To remove arrows from categories that have no children
    CheckNodesCategories(myData);
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

function RemoveNode(nodes, name) {
    for (let i = 0; i < nodes.length; ++i) {
        if (nodes[i].text.includes(name)) {
            nodes.splice(i, 1);
            break
        }
        if (nodes[i].hasOwnProperty('nodes')) {
            RemoveNode(nodes[i].nodes, name);
        }
    }
}

function CheckNodesCategories(nodes) {
    for (let i = 0; i < nodes.length; ++i) {
        if (nodes[i].hasOwnProperty('nodes') && nodes[i].nodes.length == 0) {
            delete nodes[i].nodes;
        }
        if (nodes[i].hasOwnProperty('nodes')) {
            CheckNodesCategories(nodes[i].nodes);
        }
    }
}