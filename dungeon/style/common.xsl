<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"
                doctype-public="-//W3C//DTD XHTML 1.1//EN"/>

    <xsl:template match="page">
        <xsl:variable name="common-style-path" select="'../style/'"/>
        <xsl:variable name="common-script-path" select="'../script/'"/>
        <xsl:variable name="local-style-path" select="'style/'"/>
        <xsl:variable name="local-script-path" select="'script/'"/>
        <html xmlns="http://www.w3.org/1999/xhtml">
            <head>
                <title>
                    <xsl:value-of select="@name"/>
                </title>
                <meta content="text/html; charset=utf-8" http-equiv="Content-Type"/>
                <link rel="stylesheet" href="{$common-style-path}pixelless.css" type="text/css"/>
                <xsl:for-each select="style">
                    <xsl:element name="link">
                        <xsl:attribute name="rel">stylesheet</xsl:attribute>
                        <xsl:attribute name="type">text/css</xsl:attribute>
                        <xsl:attribute name="href">
                            <xsl:choose>
                                <xsl:when test="@common='yes'">
                                    <xsl:value-of select="$common-style-path"/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of select="$local-style-path"/>
                                </xsl:otherwise>
                            </xsl:choose>
                            <xsl:value-of select="@src"/>
                        </xsl:attribute>
                    </xsl:element>
                </xsl:for-each>
                <xsl:for-each select="script[not(@bottom) or @bottom!='yes']">
                    <xsl:element name="script">
                        <xsl:attribute name="type">text/javascript</xsl:attribute>
                        <xsl:attribute name="src">
                            <xsl:choose>
                                <xsl:when test="@common='yes'">
                                    <xsl:value-of select="$common-script-path"/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of select="$local-script-path"/>
                                </xsl:otherwise>
                            </xsl:choose>
                            <xsl:value-of select="@src"/>
                        </xsl:attribute>
                    </xsl:element>
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
                        <xsl:copy-of select="/page/content"/>

                        <div class="empty_footer">
                        </div>
                    </div>
                </div>
                <div id="footer">
                    <div class="content">
                        &#169;
                        <xsl:value-of select="/page/copy"/>
                    </div>
                </div>
                <xsl:for-each select="script[@bottom='yes']">
                    <xsl:element name="script">
                        <xsl:attribute name="type">text/javascript</xsl:attribute>
                        <xsl:attribute name="src">
                            <xsl:choose>
                                <xsl:when test="@common='yes'">
                                    <xsl:value-of select="$common-script-path"/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of select="$local-script-path"/>
                                </xsl:otherwise>
                            </xsl:choose>
                            <xsl:value-of select="@src"/>
                        </xsl:attribute>
                    </xsl:element>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>