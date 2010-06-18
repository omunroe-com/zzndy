
postMessage("START\n");
postMessage("Real Other Thread\n");

setTimeout(function(){postMessage("END\n")}, 500);
