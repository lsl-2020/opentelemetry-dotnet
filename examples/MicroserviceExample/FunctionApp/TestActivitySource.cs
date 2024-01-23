// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;

namespace FunctionApp;

public static class TestActivitySource
{
    public static ActivitySource ActivitySource = new ActivitySource("StaticActivitySource");
}
