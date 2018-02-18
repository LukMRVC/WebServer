var myApp = {};
(function () {
    var source = new EventSource("http://localhost:1234/SSE/");
    source.onmessage = (event) => {
        console.log(event.data);

    };

    source.onerror = (error) => {
        console.log("Error: ", error);
        source.close();
    }


})(myApp);
