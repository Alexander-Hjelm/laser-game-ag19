---
layout: default
title: Technologies
---

{% for item in site.data.technologies%}
<table class="centerTable">
<tr>
<td class="centerTable"><img class="logo" src="{{ item.image }}" alt=""/></td>
<td class="centerTable">

<h1> {{item.name}} </h1>

</td>
</tr>
</table>
  {{item.description}}
{% endfor %}
