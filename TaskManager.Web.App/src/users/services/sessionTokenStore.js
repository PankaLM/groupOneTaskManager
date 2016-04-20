(function (angular) {
  'use strict';

  angular.module('taskManager')
   .factory('sessionTokenStore', ['$window', function ($window) {
     var cookies, sessionKey;

     function readCookie(name) {
       var all,
           cookie,
           i;

       if (cookies) {
         return cookies[name];
       }

       all = $window.document.cookie.split('; ');
       cookies = {};

       for (i = 0; i < all.length; i++) {
         cookie = all[i].split('=');
         cookies[cookie[0]] = cookie[1];
       }

       return cookies[name];
     }

     sessionKey = readCookie('sessionCookie');

     function SessionTokenStore() {
       if ($window.localStorage.sessionTokens) {
         this.token = JSON.parse($window.localStorage.sessionTokens)[sessionKey];
       }
     }

     SessionTokenStore.prototype.getSessionKey = function () {
       return sessionKey;
     };

     SessionTokenStore.prototype.getToken = function () {
       if (this.token) {
         return this.token.token;
       }

       return null;
     };

     SessionTokenStore.prototype.setToken = function (token) {
       var now = new Date(),
           tokens;

       if ($window.localStorage.sessionTokens) {
         tokens = JSON.parse($window.localStorage.sessionTokens);
       }

       tokens = tokens || {};

       this.token = {
         token: token,
         addedOn: now
       };
       tokens[sessionKey] = this.token;

       //remove tokens older than a week
       _.each(tokens, function (token, sessionKey) {
         if ((now - token.addedOn) > 7 * 24 * 3600 * 1000) {
           delete tokens[sessionKey];
         }
       });

       $window.localStorage.sessionTokens = JSON.stringify(tokens);
     };

     SessionTokenStore.prototype.deleteToken = function () {
       var tokens;

       if ($window.localStorage.sessionTokens) {
         tokens = JSON.parse($window.localStorage.sessionTokens);
         delete tokens[sessionKey];
         $window.localStorage.sessionTokens = JSON.stringify(tokens);
       }
     };

     return new SessionTokenStore();
   }]);
}(angular));