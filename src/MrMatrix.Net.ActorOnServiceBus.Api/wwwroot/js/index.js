(function (w) {
    var needs = ['shorts', 't-shirt', 'dress', 'jacket'];

    var needsText = ['👖', '👕', '👗','🧥'];
    var donors = ['Chet', 'Kendra', 'Melinda', 'Bard']; // 'Betty',
    var necessitous = ['Bully', 'Crone', 'Hag', 'Milkman']; //'Pirate',


    // https://stackoverflow.com/questions/494143/creating-a-new-dom-element-from-an-html-string-using-built-in-dom-methods-or-pro
    function createElementFromHTML(htmlString) {
        var div = document.createElement('div');
        div.innerHTML = htmlString.trim();

        // Change this to div.childNodes to support multiple top-level nodes.
        return div.firstChild;
    }

    function makePostCall(url, reqBody, callback) {
        var oReq = new XMLHttpRequest();
                oReq.open('POST', url);
                oReq.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
                oReq.send(JSON.stringify(reqBody));
                oReq.onreadystatechange = function (event) {
                    if (event.target.status === 200 && event.target.readyState === 4) {
                        var parsedObject = JSON.parse(event.target.responseText);
                        callback(parsedObject);
                    }
                };
    }

    function makeGetCall(url, callback) {
        var oReq = new XMLHttpRequest();
        oReq.open('GET', url);
        oReq.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        oReq.send();
        oReq.onreadystatechange = function (event) {
            if (event.target.status === 200 && event.target.readyState === 4) {
                var parsedObject = JSON.parse(event.target.responseText);
                callback(parsedObject);
            }
        };
    }

    function buildNeedLine(uiRebuild, templateChild, parentContainer, parentKey, needKey, needText, commandUrl, queryUrl) {
        var needComponent = createElementFromHTML(templateChild);
        var requestedValue = needComponent.getElementsByTagName('input')[0];
        var entered = needComponent.getElementsByTagName('span')[0];
        var balanced = needComponent.getElementsByTagName('span')[1];
        var button = needComponent.getElementsByTagName('button')[0];
        button.innerText = needText;
        button.addEventListener('click', function () {
            makePostCall(commandUrl, {
                "key": needKey,
                "quantity": requestedValue.valueAsNumber
            }, function (responseObject) {
                entered.innerText = responseObject['entered'];
                balanced.innerText = responseObject['balanced'];
            });
            requestedValue.value = '0';
        });

        uiRebuild[needKey] = function (enteredValue, balancedValue) {
            entered.innerText = enteredValue;
            balanced.innerText = balancedValue;
        }
        parentContainer.appendChild(needComponent);
    }

    function buildPersonPanel(templateParent, templateChild, parentContainer, parentKey, urlPrefix, urlSuffix) {
        var panel = createElementFromHTML(templateParent);
        panel.getElementsByTagName('h3')[0].innerText = parentKey;
        var commandUrl = urlPrefix + encodeURI(parentKey) + urlSuffix;
        var queryUrl = urlPrefix + encodeURI(parentKey) + '/balance';

        var uiRebuild = {};

        panel.getElementsByTagName('button')[0].addEventListener('click', function() {
            makeGetCall(queryUrl, function (responseObject) {
                console.log(responseObject);
                
                responseObject.balance.forEach(function (balance) {
                    if ('/registerDonation' == urlSuffix) {
                        uiRebuild[balance.key](balance.donation, balance.necessity);
                    } else {
                        uiRebuild[balance.key](balance.necessity, balance.donation);
                    }
                });
            });
        });

        for (var n in needs) {
            var needKey = needs[n];
            var needText = needsText[n];
            buildNeedLine(uiRebuild, templateChild, panel.getElementsByTagName('ul')[0], parentKey, needKey, needText, commandUrl, queryUrl);
        }
        parentContainer.appendChild(panel);
    }


    w.addEventListener('load', function () {
        var elements = w.document.getElementsByTagName('template');
        var templateParent = elements[0].innerHTML;
        var templateChild = elements[1].innerHTML;

        var containers = w.document.getElementsByClassName('container');
        var donorContainer = containers[0];
        var necessitousContainer = containers[1];
        
        for (var d in donors) {
            var donorKey = donors[d];
            buildPersonPanel(templateParent, templateChild, donorContainer, donorKey, '/Donor/donorId/','/registerDonation');
        }

        for (var n in necessitous) {
            var necessitousKey = necessitous[n];
            buildPersonPanel(templateParent, templateChild, necessitousContainer, necessitousKey, '/Necessitous/necessitiousId/', '/registerNecessity');
        }
    });

}(window));