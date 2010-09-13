try{

String.prototype.startsWith = function(text)
{
    return this.indexOf(text) == 0;
}

String.prototype.endsWith = function(text)
{
    return this.indexOf(text) == this.length - text.length;
}

function catchResponse()
{
    if(request.readyState == 4)
    {
        var text = request.responseText.split('\n');
        var waitForQuote = true;
        var quote = "";
        var quotes = [];
        for(var i=0;i<text.length;++i)  {
            var line = text[i];
            if(waitForQuote)    {
                if(line.startsWith('<p class="quote">'))
                {
                    waitForQuote = false;
                    if(line.endsWith('</p>'))    {
                        waitForQuote = true;
                        quotes.push(line);
                    }
                    else    {
                        quote += line;
                    }
                    
                }
            }
            else    {
                if(line.endsWith('</p>'))    {
                    waitForQuote = true;
                    quotes.push(quote + line);
                }
                else    {
                    quote += line;
                }
            }
        }
        WScript.Echo(quotes.length);
    }

    return false;
}


var url = 'http://bash.org/?random1';
var request;

try 
              {
                  request = WScript.CreateObject("Msxml2.XMLHTTP");
               }
         catch (e) 
            {
                  try 
                  {
                           request = WScript.CreateObject("Microsoft.XMLHTTP");
                       }
                 catch (e) {
}
               } 

request.onreadystatechange = catchResponse;

request.open('GET', url, false);
request.send(null);
}
catch(ex)
{
WScript.Echo(ex.message);
}




