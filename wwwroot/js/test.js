var Test = function () {
    var init = function () {
        alert("init");
    }

    var doSomething = function (prop) {
        debugger;
        const selectedAccount = event.target.value;
        if (!selectedAccount) return;
        //    $.post('/Tito/Event', { Token: $('#maskedTokenInput').val(), Account: selectedAccount },
        //        function (returnedData) {
        //            console.log(returnedData);
        //        }).fail(function () {
        //            console.log("error");
        //        });
        //    alert(prop);
        //}

        $.ajax({
            url: '/Tito/Event',
            type: 'post',
            data:
                JSON.stringify({ Token: $('#maskedTokenInput').val(), Account: selectedAccount }),
            /*Token: $('#maskedTokenInput').val(), Account: selectedAccount*/

            headers: {
                'Content-Type': 'application/json'
            },
            dataType: 'json',
            success: function (data) {
                console.info(data);

                const eventDropdown = document.getElementById("eventdropdown");
                eventDropdown.innerHTML = '';

                for (let i = 0; i < data.events.length; i++) {
                    let option = document.createElement("option");
                    let title = data.events[i].slug;
                    option.value = title;
                    option.textContent = title;
                    eventDropdown.appendChild(option);
                }
            }
        })
    };

    return {
        init: init,
        doSomething: doSomething
    };
}();

//$(function () {
//    Test.init
//})

//async function getEvents(event) {
//    const selectedAccount = event.target.value;
//    if (!selectedAccount) return;
//    const errorMessage = document.getElementById("errorMessage");

//    try {
//        const response = await fetch('/Tito/Event', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json'
//            },
//            body: JSON.stringify({ Token: '@Model.TitoSettings.Token', Account: selectedAccount })
//        });

//        if (response.ok) {
//            const result = await response.json();
//            // Update the events dropdown with the fetched events

//            const eventDropdown = document.getElementById("eventdropdown");
//            eventDropdown.innerHTML = '';
//            result.events.forEach(event => {
//                const option = document.createElement("option");
//                option.value = event;
//                option.textContent = event.Title;
//                eventDropdown.appendChild(option);
//            });
//        } else {
//            const result = await response.json();
//            errorMessage.textContent = result.error || "Please try with a valid account.";
//        }
//    } catch (error) {
//        errorMessage.textContent = "An error occurred. Please try again.";
//    }
//}
