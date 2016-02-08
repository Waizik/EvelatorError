'use strict';

// jedna se o kontrolery z modulu default, neni zde pouzit zadny plugin (v komentari je plugin pro prekladani textu)
angular.module('default', /*['translate']*/ [ 'ui.bootstrap.datetimepicker'])
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
            $scope.filter = {};
            // funkce pro generovani textu
            $scope.date = {};

            $scope.tab = 1;

            // startGenerating je priznak, zda se ma zacit nebo skoncit
      
            $scope.setTab = function(setTab){
                $scope.tab = setTab;
            }

            $scope.isSet = function(checkTab){
                return $scope.tab === checkTab;
            }

            $scope.filtrTable = function(filter) {
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
                                                                              locality: $scope.locality,
                                                                              dateFrom : $scope.date.dateFrom, //toto zde nema vyznam, protoze timestamp je vzdy maximalni cas. Az vypisu 
                                                                              dateTo : $scope.date.dateTo //konkretnich chyb. Nicmene fahci to. (V apicontroleru se vytisl dany cas)
                                                                              })
                        .success(function(data){
                            console.log(data);
                            $scope.messagesGenerated++;
                            $scope.texts = data;
                        })
                        .error(function(data) {
                             bootbox.alert("Nekde se stala chyba: " + data, function() {

                            });
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
                             bootbox.alert("Nekde se stala chyba: " + data, function() {

                            });
                        });

                       


                }
            }, 1000);

             $scope.MyTableClass = function(text){
                            if(text.Errors[0] !== 0)
                                return "danger";
                            else if (text.IsNull)
                                return "warning";
                            else     
                                return "success";
            }

            $scope.commas = function(items) {
                return items.join(",");
            }    

            $scope.updateTable = function(){
                $http.post('http://localhost:8089/api/updateEvelator/', {postCode: $scope.postCode,
                                                                              street: $scope.street,
                                                                              number: $scope.number,
                                                                              locality: $scope.locality,
                                                                              newEvelatorID: $scope.newEvelatorID,
                                                                              evelatorID : $scope.evelatorID                                                                           
                                                                              })
                        .success(function(data){
                            console.log(data);
                        })
                        .error(function(data) {                            
                            bootbox.alert("Nekde se stala chyba: " + data, function() {

                            });
                        });
            }


            $scope.getIdInfo = function(){
                console.log("volam get info");
                if($scope.evelatorID != null && $scope.evelatorID != ""){
                    $http.post('http://localhost:8089/api/getIdInfo/', $scope.evelatorID)
                        .success(function(data){
                            console.log("volam get info C#");
                            //console.log(data);
                            $scope.street = data.Street;
                            $scope.postCode = data.Postcode;
                            $scope.number = data.Number;
                            $scope.locality = data.Locality;
                        })
                        .error(function(data){
                            bootbox.alert("Nekde se stala chyba: " + data, function() {

                            });
                        });
                }
                else{
                     $scope.street = "";
                            $scope.postCode = "";
                            $scope.number = "";
                            $scope.locality = "";
                }
            }
                

            // vyuziti $timeout pro zobrazeni textu po 3 vterinach po nacteni stranky
            //var timeout = $timeout(function() {
                // funkce, ktera se provede
              //  $scope.texts.push('Text vygenerovany pomoci $timeout.');
                // 3000 ms - po 3 s se text vlozi
            //}, 3000);
        }
    ])
    
    /*.controller('ValidationController', ['$scope', '$http',
        function ($scope, $http) {
           
          
        }
    ])*/

   