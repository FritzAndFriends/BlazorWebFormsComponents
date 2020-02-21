(function(){

  var Page = {

    setTitle: function(title) {
      
			document.title = title;

    },

    getTitle: function() {

      return document.title;

    }

  }

  window.bwfc = window.bwfc ?? {};
  window.bwfc.Page = Page;

})();
