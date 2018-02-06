
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
