MessageBox = function()
{
    this.attached = false;

    this.dom = null;
    this.title = null;
    this.body = null;
    this.status = null;
};

var MB = MessageBox.prototype;

MB.show = function(title, body, actions)
{
    function fn(parent, element)
    {
        var name = element.title.toLowerCase().replace(/^\W+|\W+$/, '').replace(/\W+/, '-');
        var title = element.title;
        var click = (element.click || (name + '()')) + ';return false;';
        var url = '#' + name;
        var id = element.id || '';

        var elt = $div({'class':name + ' ' + (element.class || '')}, $a({id:id, title:title, onclick:click, href:url}, title));
        parent.appendChild(elt);
        return parent;
    }

    if (this.dom == null)
    {
        this.title = $div({'class':'title', 'id':'title'});
        this.body = $p({'class':'message','id':'msg'});
        this.status = $div({'id':'status', 'class':'cancel'});

        this.dom = $div({'class':'popup'}, [
            $div({'class':'content'}, [
                this.title,
                this.body,
                this.status
            ])]);
    }

    this.title.innerHTML = title;

    if (typeof body == 'string')
    {
        this.body.innerHTML = body;
    }
    else
    {
        this.body.innerHTML = '';
        this.body.appendChild(body);
    }


    this.status.innerHTML = '';

    if (actions instanceof Array)
        actions.reduce(fn, this.status);

    else fn(this.status, actions);

    if (!this.attached) {
        document.body.appendChild(this.dom);
        this.attached = true;
    }
};

MB.hide = function()
{
    if (this.attached)
    {
        this.dom.parentNode.removeChild(this.dom);
        this.attached = false;
    }

};

