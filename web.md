# Web #

---

## google ##
```
*web google <number> <text>
```
  * Google search: you specify number of results 1-10 in 

&lt;number&gt;

 and the text to find: 

&lt;text&gt;

 . Bot will return as much results as he found (due to 

&lt;number&gt;

).
**Example:**
```
user: *web google 1 agsxmpp
bot: AG-Software - agsXMPP SDKAG-Software - , open-source cross platform XMPP Library.
www.ag-software.de/content/agsxmpp/ 
```

---

## yandex ##
```
*web yandex <number> <text>
```
  * Yandex search: you specify number of results 1-10 in 

&lt;number&gt;

 and the text to find: 

&lt;text&gt;

 . Bot will return as much results as he found (due to 

&lt;number&gt;

).
**Example:**
```
user: *web yandex 1 agsxmpp
bot: AG-Software - agsXMPP SDKAG-Software - , open-source cross platform XMPP Library.
www.ag-software.de/content/agsxmpp/ 
```

---

## xep ##
```
*web xep <tip>
```
  * A XEP (XMPP Extensions Protocol) search (see http://xmpp.org/extensions) by a piece of text (number) - 

&lt;tip&gt;


**Example:**
```
user: *web xep 99
bot: XEP 0099:
IQ Query Action Protocol Type: Standards Track Status: Deferred
http://www.xmpp.org/extensions/xep-0099.html

user:*web xep iq query
bot: XEP 0099:
IQ Query Action Protocol Type: Standards Track Status: Deferred
http://www.xmpp.org/extensions/xep-0099.html
```

---

## gt ##
```
*web gt <lang_pair> <text>
```
  * Google Translator, which lets to translate any Unicode phrase from one language to another. Use language-pair 

<lang\_pair>

 and 

&lt;text&gt;

 to set the text to translate.
**Example:**
```
user: *web gt ed|Hi, friend, how do you feel?
bot: Hallo, Freund, wie fühlst du dich?
```
**P.S.** None of author and bot does not take any responsibility for the translation results: translating is fully handled by http://translate.google.com

_See **gtlangs**_

---

## gtlangs ##
```
*web gtlangs
```
  * Shows a list of all language-pairs availablefor a **gt** translation.
  * No parameters needed
_See **gt**_

---

## news ##
```
*web web news <number>
```
  * Displsy a Google News with 

&lt;number&gt;

 of news.

---

## worldnews ##
```
*web web worldnews <number>
```
  * Displsy a Google News with 

&lt;number&gt;

 of news.


---

## bor ##
```
*web web worldnews <number>
```
  * Displsy a quote from http://bash.org.ru.


---

## anekdot ##
```
*web web anekdot
```
  * Displsy a joke from http://anekdot.ru