/*
 Copyright (c) 2013 yvt
 
 This file is part of OpenSpades.
 
 OpenSpades is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 OpenSpades is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with OpenSpades.  If not, see <http://www.gnu.org/licenses/>.
 
 */


uniform sampler2D texture_;
uniform sampler2D cocTexture;

varying vec2 texCoord;
uniform vec2 offset;

vec4 doGamma(vec4 col) {
#if !LINEAR_FRAMEBUFFER
	col.xyz *= col.xyz;
#endif
	return col;
}

vec4 sampleDoF(vec2 at) {
	vec4 color = doGamma(texture2D(texture_, at));
	color.w = texture2D(cocTexture, at).x + 0.001;
	color.xyz *= color.w;
	return color;
}

void main() {
	
	float coc = texture2D(cocTexture, texCoord).x;
	vec4 v = vec4(0.);
	
	vec4 offsets = vec4(0., 0.25, 0.5, 0.75) * coc;
	vec4 offsets2 = offsets + coc * 0.125;
	
	v += sampleDoF(texCoord);
	v += sampleDoF(texCoord + offset * offsets.y);
	v += sampleDoF(texCoord + offset * offsets.z);
	v += sampleDoF(texCoord + offset * offsets.w);
#if 1
	v += sampleDoF(texCoord + offset * offsets2.x);
	v += sampleDoF(texCoord + offset * offsets2.y);
	v += sampleDoF(texCoord + offset * offsets2.z);
	v += sampleDoF(texCoord + offset * offsets2.w);
	v.xyz *= 1. / v.w;
#else
    v *= 0.25;
#endif
#if !LINEAR_FRAMEBUFFER
	v.xyz = sqrt(v.xyz);
#endif
	
	gl_FragColor = v;
	gl_FragColor.w = 1.;
}

