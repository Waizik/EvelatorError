'use strict';

// jedna se o kontrolery z modulu default, neni zde pouzit zadny plugin (v komentari je plugin pro prekladani textu)
angular.module('default', /*['translate']*/ ['ui.bootstrap.datetimepicker'])
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
            // kolik bylo vygenerovani zprav
            $scope.messagesGenerated = 0;
            // priznak, zda se generuje lokalni
            $scope.filterOn = false;
            // funkce pro generovani textu
            // startGenerating je priznak, zda se ma zacit nebo skoncit
      

            $scope.generateText = function(filter) {
                $scope.filterOn = filter;
            };
       
             
            // vyuziti $interval pro generovani lokalniho textu po 1 vterine
            //jedno pro generovani s filtrem a druhe (dole pro generovani bez filtru)
            var intervalLocal = $interval(function() {
                // provadet pouze, pokud mame generovat
           
                if ($scope.filterOn) {
                         
                    $http.post('http://localhost:8089/api/getFilterErrors/', {postCode: $scope.postCode,
                                                                              street: $scope.street,
                                                                              number: $scope.number,
                                                                              locality: $scope.locality})
                        .success(function(data){
                            console.log(data);
                            $scope.messagesGenerated++;
                            $scope.texts = data;
                        })
                        .error(function(data) {
                            alert(data);
                        });
                    
                }
            }, 1000);
            
            
            // vyuziti $interval pro generovani textu z webu
            var intervalOnline = $interval(function() {
               // provadet pouze, pokud mame generovat
                if (!$scope.filterOn) {
                    // pozadavek
                    $http.get('http://localhost:8089/api/getErrors')
                        // pokud se povede
                        .success(function (data) {
                            console.log(data);
                            // inkrementace pocut generovanych zprav
                            $scope.messagesGenerated++;
                            // funkce, ktera se provede
                             $scope.texts =  data;                                           
                        })
                        // kdyz se nepovede
                        .error(function (data) {
                            alert(data);
                        });

                       


                }
            }, 1000);

             $scope.MyTableClass = function(text){
                            if(text.ErrorCode > 0)
                                return "danger";
                            else if (text.IsNull)
                                return "warning";
                            else     
                                return "success";
                        }    
                

            // vyuziti $timeout pro zobrazeni textu po 3 vterinach po nacteni stranky
            //var timeout = $timeout(function() {
                // funkce, ktera se provede
              //  $scope.texts.push('Text vygenerovany pomoci $timeout.');
                // 3000 ms - po 3 s se text vlozi
            //}, 3000);
        }
    ])
    
    .controller('ViewController', ['$scope', '$http', '$interval', '$timeout',
        function ($scope, $http, $interval, $timeout) {
        }
    ]);

   