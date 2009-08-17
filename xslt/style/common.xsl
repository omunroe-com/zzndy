<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"
                doctype-public="-//W3C//DTD XHTML 1.1//EN"/>
    <xsl:template match="page">
        <html xmlns="http://www.w3.org/1999/xhtml">
            <head>
                <meta content="text/html; charset=utf-8" http-equiv="Content-Type"/>
                <link rel="stylesheet" href="../style/pixelless.css" type="text/css"/>
                <xsl:for-each select="style[@href]">
                    <link rel="stylesheet" href="{@href}" type="text/css"/>
                </xsl:for-each>

                <xsl:for-each select="script[@href and @rel!='foot']">
                    <script type="text/javascript" src="{@href}"></script>
                </xsl:for-each>
                <title>
                    <xsl:value-of select="@name"/>
                </title>
                <!-- Place for styles -->
                <xsl:for-each select="style[@type='text/css']">
                    <xsl:copy-of select="."/>
                </xsl:for-each>
            </head>
            <body>
                <div id="container">
                    <div class="content header">
                        <h1>
                            <xsl:value-of select="@name"/>
                        </h1>
                    </div>

                    <div class="content">
                        <canvas id="target" width="800px" height="500px">Convas is not supported in this browser.
                        </canvas>
                        <div class="empty_footer">
                        </div>
                    </div>
                </div>
                <div id="footer">
                    <div class="content">
                        &#169; Copyright
                    </div>
                </div>
                <!-- Place for scripts -->
                <xsl:for-each select="script[@href and @rel='foot']">
                    <script type="text/javascript" src="{@href}"></script>
                </xsl:for-each>
                 <xsl:for-each select="script[@type='text/javascript']">
                    <xsl:copy-of select="."/>
                </xsl:for-each>
            </body>
        </html>


    </xsl:template>
</xsl:stylesheet>