(function(){

  var Page = {

    setTitle = function(title) {
      
      window.title = title;

    },

    getTitle = function() {

      return window.title;

    }

  }

  window.bwfc = window.bwfc ?? {};
  window.bwfc.Page = Page;

})();