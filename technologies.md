---
layout: default
title: Technologies
---

Our technological choices were based mainly on experience.
- - - - 

{% for item in site.data.technologies%}
<table class="centerTable">
<tr>
<td class="centerTable"><img class="logo" src="{{ item.image }}" alt=""/></td>
<td class="centerTable">

<h1 style="color:#0090ff;"> {{item.name}} </h1>

</td>
</tr>
</table>
  {{item.description}}
{% endfor %}
