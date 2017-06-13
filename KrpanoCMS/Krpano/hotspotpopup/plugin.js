function krpanoplugin() {
	
	// the krpano and plugin interface objects
	var krpano;
	var plugin;
	var layers = [];
	var i = 0;
	
	// registerplugin - startup point for the plugin (required)
	// - krpanointerface = krpano interface object
	// - pluginpath = the fully qualified plugin name (e.g. "plugin[name]")
	// - pluginobject = the xml plugin object itself
	this.registerplugin = function(krpanointerface, pluginpath, pluginobject) {
		krpano = krpanointerface;
		plugin = pluginobject;
		krpano.events.onclick = _handleClick.bind(this);
		_loadStyles();
		_createHotspots();
	};

	// unloadplugin - exit point for the plugin (optionally)
	// - will be called from krpano when the plugin will be removed
	// - everything that was added by the plugin should be removed here
	this.unloadplugin = function() {
		plugin = null;
		krpano = null;
		layers = null;
		i = 0;
		krpano.events.onclick = null;
	};
	
	function _loadStyles() {
		var styleTag = document.createElement('link');
	  	styleTag.setAttribute('rel', 'stylesheet');
	  	styleTag.setAttribute('href', plugin.url.replace('.js', '.css'));
	  	plugin.sprite.appendChild(styleTag);
	}
	
	function _createHotspots() {
		var data = JSON.parse(krpano.data.getItem(plugin.hotspots).content);
		data.map(function(item, index) {
			item.name = 'hotspot-' + index;
			_createHotspot(item);
		});
	}
	
	function _createHotspot(data) {
		var hotspot = krpano.addhotspot(data.name);
		hotspot.atv = data.atv;
		hotspot.ath = data.ath;
		hotspot.content = data.content;
		hotspot.distorted = true;
		hotspot.zorder = 10;
		hotspot.url = plugin.url.replace('plugin.js', 'pin.png');
		hotspot.onclick = _openPopup.bind(hotspot);
	}
	
	function _openPopup() {
		var hotspot = this;
		krpano.call("looktohotspot(" + hotspot.name + ")");
		hotspot.zorder = 20;
		_closeAllPopups();
		
		var layer = krpano.addlayer(hotspot.name);
		layer.type = 'text';
		layer.onclick = _closePopup.bind(layer);
		layer.parent = hotspot.getfullpath();
		layers.push(layer);
		
		var popup = document.createElement('div');
		popup.classList.add('popup');
		popup.innerHTML = hotspot.content;
		
		layer.sprite.appendChild(popup);
	}
	
	function _closePopup(layer) {
		var target = layer || this;
		layers.splice(layers.indexOf(target), 1);
		krpano.get(target.parent).zorder = 10;
		krpano.removelayer(target.name);
	}
	
	function _closeAllPopups() {
		layers.map(_closePopup)
	}
	
	function _handleClick() {
		var coords = krpano.screentosphere(krpano.mouse.x, krpano.mouse.y);
		var input = prompt('Add content for the hotspot', 'Wow!');
		if (!input) return;
		var hotspotData = {
			name: 'click' + (++i),
			ath: coords.x,
			atv: coords.y,
			content: input
		};
		_createHotspot(hotspotData);
		// @TODO: AJAX POST hotspotData
	}
}