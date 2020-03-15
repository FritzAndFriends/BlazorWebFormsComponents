(function(){

  var Page = {

    setTitle: function(title) {

			document.title = title;

    },

    getTitle: function() {

      return document.title;

    },

		OnAfterRender: function() {
			console.log("Running Window.load function");
			var elementsToReplace = document.querySelectorAll("*[onclientclick]");
			for (var el of elementsToReplace) {
				console.log(el.getAttribute("onclientclick"));
				el.addEventListener('click', function(e) { eval(e.target.getAttribute('onclientclick'))});
			}
		}

  }

  window.bwfc = window.bwfc ?? {};
  window.bwfc.Page = Page;

})();
