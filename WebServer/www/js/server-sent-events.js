var myApp = {};
(function () {
    var url = window.location.host;
    AddOrder = function (order) {

        let ordertext = "";
        for (let i = 0; i < order.OrderFood.length; ++i) {
            ordertext += order.OrderFood[i].FoodCount + "x " + order.OrderFood[i].Food.Name + ", ";
        }
        ordertext = ordertext.substring(0, ordertext.length - 1);


        //of course not
        //uživatel, objednávka, cena
        let element = $("<tr></tr>");
        element.append($("<td></td>").text(order.Id));
        element.append($("<td></td>").text(ordertext));
        element.append($("<td></td>").text(order.TotalPrice + " Kč"));
        $("#orders").prepend(element);
    }



    var source = new EventSource("http://" + url + "/SSE/");
    source.onmessage = (event) => {
        AddOrder(JSON.parse(event.data));
    };

    source.onerror = (error) => {
        console.log("Error: ", error);
        source.close();
    }


})(myApp);
