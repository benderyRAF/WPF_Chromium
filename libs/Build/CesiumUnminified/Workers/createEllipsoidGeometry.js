/**
 * Cesium - https://github.com/CesiumGS/cesium
 *
 * Copyright 2011-2020 Cesium Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Columbus View (Pat. Pend.)
 *
 * Portions licensed separately.
 * See https://github.com/CesiumGS/cesium/blob/master/LICENSE.md for full licensing details.
 */

define(['./when-54c2dc71', './EllipsoidGeometry-8f202aa1', './GeometryOffsetAttribute-b31a3ff4', './Check-6c0211bc', './Transforms-c00dcfdd', './Cartesian2-15709417', './Math-fbfbe3b1', './RuntimeError-2109023a', './ComponentDatatype-a26dd044', './WebGLConstants-76bb35d1', './GeometryAttribute-c2f3a6a0', './GeometryAttributes-4fcfcf40', './IndexDatatype-eaebd63b', './VertexFormat-4d8b817a'], function (when, EllipsoidGeometry, GeometryOffsetAttribute, Check, Transforms, Cartesian2, _Math, RuntimeError, ComponentDatatype, WebGLConstants, GeometryAttribute, GeometryAttributes, IndexDatatype, VertexFormat) { 'use strict';

  function createEllipsoidGeometry(ellipsoidGeometry, offset) {
    if (when.defined(offset)) {
      ellipsoidGeometry = EllipsoidGeometry.EllipsoidGeometry.unpack(ellipsoidGeometry, offset);
    }
    return EllipsoidGeometry.EllipsoidGeometry.createGeometry(ellipsoidGeometry);
  }

  return createEllipsoidGeometry;

});
//# sourceMappingURL=createEllipsoidGeometry.js.map
