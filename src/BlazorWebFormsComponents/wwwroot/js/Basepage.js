(function(){

  var Page = {

    setTitle: function(title) {

			document.title = title;

    },

    getTitle: function() {

      return document.title;

    },

		OnAfterRender: function() {
			console.debug("Running Window.load function");
			FormatClientClick();
		},

		AddScriptElement: function(location, callback) {
			var el = document.createElement("script");
			el.setAttribute("src", location);
			document.head.appendChild(el);

			if (callback != null) {
				el.addEventListener("load", function() { eval(callback); });
			}

		},

  };

	var FormatClientClick = function() {
			var elementsToReplace = document.querySelectorAll("*[onclientclick]");
			for (var el of elementsToReplace) {
				if (!el.getAttribute("data-onclientclick")) {
					console.debug(el.getAttribute("onclientclick"));
					el.addEventListener('click', function(e) { eval(e.target.getAttribute('onclientclick'))});
					el.setAttribute("data-onclientclick", "1");
				}
			}
	};

  window.bwfc = window.bwfc ?? {};
  window.bwfc.Page = Page;

})();
