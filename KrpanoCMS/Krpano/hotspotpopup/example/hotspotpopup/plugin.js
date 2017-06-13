function krpanoplugin() {
	
	// the krpano and plugin interface objects
	var krpano;
	var plugin;
	
	// registerplugin - startup point for the plugin (required)
	// - krpanointerface = krpano interface object
	// - pluginpath = the fully qualified plugin name (e.g. "plugin[name]")
	// - pluginobject = the xml plugin object itself
	this.registerplugin = function(krpanointerface, pluginpath, pluginobject) {
		krpano = krpanointerface;
		plugin = pluginobject;
		loadStyles();
		createHotspots();
	}

	// unloadplugin - exit point for the plugin (optionally)
	// - will be called from krpano when the plugin will be removed
	// - everything that was added by the plugin should be removed here
	this.unloadplugin = function() {
		plugin = null;
		krpano = null;
	}

	// onresize (optionally)
	// - width,height = the new size for the plugin
	// - when not defined then only the krpano plugin html element will be sized
	this.onresize = function(width, height) {
		// not used in this example
		// the plugin content will resize automatically because
		// of the width=100%, height=100% CSS style
		return false;
	}
	
	function loadStyles() {
		var styleTag = document.createElement('link');
	  	styleTag.setAttribute('rel', 'stylesheet');
	  	styleTag.setAttribute('href', plugin.url.replace('.js', '.css'));
	  	plugin.sprite.appendChild(styleTag);
	}
	
	function createHotspots() {
		var data = JSON.parse(krpano.data.getItem(plugin.hotspots).content);
		data.map(function(item, index) {
			var hotspot = krpano.addhotspot('hotspot-' + index);
			hotspot.atv = item.atv;
			hotspot.ath = item.ath;
			hotspot.content = item.content;
			hotspot.distorted = true;
			hotspot.url = plugin.url.replace('plugin.js', 'pin.png');
			hotspot.onclick = openPopup.bind(hotspot);
		});
	}
	
	function openPopup() {
		var hotspot = this;
		krpano.call("looktohotspot(" + hotspot.name + ")");
		
		var layer = krpano.addlayer(hotspot.name);
		layer.type = 'text';
		layer.onclick = closePopup.bind(layer);
		layer.parent = hotspot.getfullpath();
		
		var popup = document.createElement('div');
		popup.classList.add('popup');
		popup.innerHTML = hotspot.content;
		
		layer.sprite.appendChild(popup);
	}
	
	function closePopup() {
		krpano.removelayer(this.name);
	}
}