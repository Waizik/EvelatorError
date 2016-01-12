'use strict';

// jedna se o kontrolery z modulu default, neni zde pouzit zadny plugin (v komentari je plugin pro prekladani textu)
angular.module('default', /*['translate']*/ [])
    // toto se spusti hned na zacatku
    // nemusi se vubec pouzivat, jestli neni potreba
    .run(function ($rootScope, $location, $window, $timeout, $http, $q) {
    })
    // definice pouzitych modulu a funkci to v zavorce MUSI odpovidat argumentum ve function
    // ty s dolarem jsou moduly a fce z Angularu nebo jeho rozsireni
    // ty bez dolaru jsou uzivatelem definovane
    // $scope - zakladni prvek daneho kontroleru
    // $http - pouziti pro dotazy POST a GET a dalsi
    // $interval - interval, ve kterem se da periodicky neco delat
    // $timeout - za jakou dobu od spusteni (nacteni stranky atd.) se ma jaka akce provest jednou
    // config - pokud byste pouzival nejaky konfiguracni soubor (napr. s vychozimi hodnotami)
    
    .controller('IndexController', ['$scope', '$http', '$interval', '$timeout',
        function ($scope, $http, $interval, $timeout) {
            
            // $scope.funkce/$scope.promenna bude funkce/promenna dostupna i ve view
            // prazdne pole s texty
            $scope.texts = [];
            $scope.errors = errorr;
            // kolik bylo vygenerovani zprav
            $scope.messagesGenerated = 0;
            // priznak, zda se generuje lokalni
            $scope.generatingLocal = false;
            // priznak, zda se generuje online
            $scope.generatingOnline = false;
            // funkce pro generovani textu
            // startGenerating je priznak, zda se ma zacit nebo skoncit
            $scope.generateText = function(startGenerating, localOrOnline) {
                if (localOrOnline)
                    $scope.generatingLocal = startGenerating;
                else
                    $scope.generatingOnline = startGenerating;
            };
            
            // vyuziti $interval pro generovani lokalniho textu po 1 vterine
            var intervalLocal = $interval(function() {
                // provadet pouze, pokud mame generovat
                if ($scope.generatingLocal) {
                    // inkrementace pocut generovanych zprav
                    $scope.messagesGenerated++;
                    // funkce, ktera se provede
                    $scope.texts.push('Text ' + $scope.messagesGenerated + ' vygenerovany pomoci $interval.');
                }
            }, 1000);
            
            
            // vyuziti $interval pro generovani textu z webu
            var intervalOnline = $interval(function() {
                // provadet pouze, pokud mame generovat
                if ($scope.generatingOnline) {
                    // pozadavek
                    $http.get('http://jsonplaceholder.typicode.com/posts/1')
                        // pokud se povede
                        .success(function (data) {
                            // inkrementace pocut generovanych zprav
                            $scope.messagesGenerated++;
                            // funkce, ktera se provede
                            $scope.texts.push('Text ' + ' vygenerovany pomoci $interval online: {userId: '+ data.userId + ', id: '+ data.id + ', title: '+ data.title + ', body: '+ data.body + '}');
                            errors.id.push(data.id);
                            errors.userId.push(data.userId);
                            errors.body.push(data.body);
                            errors.title.push(data.title);
                      
                        })
                        // kdyz se nepovede
                        .error(function (data) {
                            alert(data);
                        });
                }
            }, 1000);
            
            // vyuziti $timeout pro zobrazeni textu po 3 vterinach po nacteni stranky
            var timeout = $timeout(function() {
                // funkce, ktera se provede
                $scope.texts.push('Text vygenerovany pomoci $timeout.');
                // 3000 ms - po 3 s se text vlozi
            }, 3000);
        }
    ])
    
    .controller('ViewController', ['$scope', '$http', '$interval', '$timeout',
        function ($scope, $http, $interval, $timeout) {
        }
    ]);

    var errorr = [{
                    userId: '1',
                    id: '2',
                    body: '3',
                    title: '4'
                }];